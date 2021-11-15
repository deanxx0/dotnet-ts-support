using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnet_ts_support.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace dotnet_ts_support.Controllers
{
    [Route("")]
    [ApiController]
    [Authorize]
    public class TrainController : ControllerBase
    {
        private readonly DirectoryService _directoryService;
        private readonly TrainSettingService _trainSettingService;
        private readonly TrainService _trainSerivce;
        private readonly TrainServerInfoService _trainServerInfoService;

        public TrainController(
            DirectoryService directoryService,
            TrainSettingService trainSettingService,
            TrainService trainService,
            TrainServerInfoService trainServerInfoService
            )
        {
            _directoryService = directoryService;
            _trainSettingService = trainSettingService;
            _trainSerivce = trainService;
            _trainServerInfoService = trainServerInfoService;
        }

        [AllowAnonymous]
        [HttpPost("trainServerInfo")]
        public ActionResult<TrainServerInfo> CreateTrainServerInfo([FromBody] TrainServerInfo trainServerInfo)
        {
            _trainServerInfoService.Create(trainServerInfo);
            return trainServerInfo;
        }

        [HttpPost("train")]
        public ActionResult<ApiResponseModel> StartTrain([FromBody] StartTrainModel startTrainModel)
        {
            var directory = buildDirectory(startTrainModel);
            _directoryService.Create(directory);
            var trainSetting = buildTrainSetting(startTrainModel);
            _trainSettingService.Create(trainSetting);
            var trainServerInfo = _trainServerInfoService.Get(startTrainModel.serverIndex);
            var trainRequestModel = buildTrainRequestModel(startTrainModel);
            var responseTrain = _trainSerivce.PostTrainToServer(trainServerInfo.uri, trainRequestModel).Result;
            var train = buildTrain(
                startTrainModel.serverIndex,
                startTrainModel.name,
                responseTrain.id,
                directory.id,
                trainSetting.id
                );
            _trainSerivce.Create(train);
            return new ApiResponseModel(true, train);
        }

        [HttpGet("train/totalcount")]
        public ActionResult<ApiResponseModel> GetTotalCount()
        {
            var totalCount = _trainSerivce.GetTotalCount().Result;
            return new ApiResponseModel(true, totalCount);
        }

        [HttpGet("train/pages/{pageNo}")]
        public ActionResult<ApiResponseModel> GetTrainInfoPage(int pageNo, [FromQuery] int perPage = 5)
        {
            var trains = _trainSerivce.GetTrainPage(pageNo, perPage);
            List<TrainInfoModel> trainInfos = new();
            foreach(Train train in trains)
            {
                var trainServerInfo = _trainServerInfoService.Get(train.serverIndex);
                var trainStatus = _trainSerivce.GetStatusFromServer(trainServerInfo.uri, train.serverTrainId).Result;
                var trainMetric = _trainSerivce.GetMetricFromServer(trainServerInfo.uri, train.serverTrainId).Result;
                var trainInfo = buildTrainInfoModel(train, trainStatus, trainMetric);
                trainInfos.Add(trainInfo);
            }
            //return trainInfos.ToArray();
            return new ApiResponseModel(true, trainInfos.ToArray());
        }
        
        [HttpGet("directory/{trainId}")]
        public ActionResult<ApiResponseModel> GetDirectory(string trainId)
        {
            var train = _trainSerivce.Get(trainId);
            if (train == null) return NotFound();
            var directory = _directoryService.Get(train.directoryId);
            if (directory == null) return NotFound();
            //return directory;
            return new ApiResponseModel(true, directory);
        }

        [HttpGet("train/setting/{trainId}")]
        public ActionResult<ApiResponseModel> GetTrainSetting(string trainId)
        {
            var train = _trainSerivce.Get(trainId);
            if (train == null) return NotFound();
            var trainSetting = _trainSettingService.Get(train.trainSettingId);
            if (trainSetting == null) return NotFound();
            var trainSettingResponse = buildTrainSettingResponseModel(train, trainSetting);
            return new ApiResponseModel(true, trainSettingResponse);
        }

        [HttpGet("local/dataset")]
        public ActionResult<ApiResponseModel> GetDataset()
        {
            var datasets = _directoryService.GetDatasets();
            return new ApiResponseModel(true, datasets);
        }

        [HttpGet("local/pretrain")]
        public ActionResult<ApiResponseModel> GetPretrain()
        {
            var pretrains = _directoryService.GetPretrains();
            return new ApiResponseModel(true, pretrains);
        }

        [HttpGet("train-server/resource")]
        public ActionResult<ApiResponseModel> GetResources()
        {
            var serverIndex = new int[] { 0, 1, 2, 3 };
            List<ResourceModel> resources = new();
            foreach (int i in serverIndex)
            {
                var serverUri = _trainServerInfoService.Get(i);
                var resource = _trainSerivce.GetResources(serverUri.uri).Result;
                resources.Add(resource);
            }
            //return resources.ToArray();
            return new ApiResponseModel(true, resources.ToArray());
        }

        [HttpGet("train/{id}")]
        public ActionResult<ApiResponseModel> GetTrainInfo(string id)
        {
            var train = _trainSerivce.Get(id);
            if (train == null) return NotFound();
            var trainServerInfo = _trainServerInfoService.Get(train.serverIndex);
            if (trainServerInfo == null) return NotFound();
            var trainStatus = _trainSerivce.GetStatusFromServer(trainServerInfo.uri, train.serverTrainId).Result;
            if (trainStatus == null) return NotFound();
            var trainMetric = _trainSerivce.GetMetricFromServer(trainServerInfo.uri, train.serverTrainId).Result;
            if (trainMetric == null) return NotFound();
            var trainInfo = buildTrainInfoModel(train, trainStatus, trainMetric);
            //return trainInfo;
            return new ApiResponseModel(true, trainInfo);
        }

        [HttpDelete("train/{id}")]
        public IActionResult DeleteTrain(string id)
        {
            var train = _trainSerivce.Get(id);
            if (train == null) return NotFound();
            var trainServerInfo = _trainServerInfoService.Get(train.serverIndex);
            if (trainServerInfo == null) return NotFound();
            var trainStatus = _trainSerivce.GetStatusFromServer(trainServerInfo.uri, train.serverTrainId).Result;
            if (trainStatus != "done" && trainStatus != "error")
            {
                Console.WriteLine("Training is on processing : only 'done' or 'error' train status could be deleted");
                return NoContent();
            }
            _directoryService.Remove(train.directoryId);
            _trainSettingService.Remove(train.trainSettingId);
            _trainSerivce.Remove(id);
            return NoContent();
        }

        private Train buildTrain(int serverIndex, string name, string serverTrainId, string directoryId, string trainSettingId)
        {
            return new Train()
            {
                id = ObjectId.GenerateNewId().ToString(),
                serverIndex = serverIndex,
                name = name,
                serverTrainId = serverTrainId,
                directoryId = directoryId,
                trainSettingId = trainSettingId
            };
        }

        private Directory buildDirectory(StartTrainModel startTrainModel)
        {
            return new Directory()
            {
                id = ObjectId.GenerateNewId().ToString(),
                directories = startTrainModel.directories
            };
        }

        private TrainSetting buildTrainSetting(StartTrainModel startTrainModel)
        {
            return new TrainSetting()
            {
                id = ObjectId.GenerateNewId().ToString(),
                batchSize = startTrainModel.batchSize,
                pretrainData = startTrainModel.pretrainData,
                width = startTrainModel.width,
                height = startTrainModel.height,
                channels = startTrainModel.channels,
                baseLearningRate = startTrainModel.baseLearningRate,
                gamma = startTrainModel.gamma,
                stepCount = startTrainModel.stepCount,
                maxIteration = startTrainModel.maxIteration,

                mirror = startTrainModel.mirror,
                flip = startTrainModel.flip,
                rotation90 = startTrainModel.rotation90,
                zoom = startTrainModel.zoom,
                tilt = startTrainModel.tilt,
                shift = startTrainModel.shift,
                rotation = startTrainModel.rotation,
                contrast = startTrainModel.contrast,
                brightness = startTrainModel.brightness,
                smoothFiltering = startTrainModel.smoothFiltering,
                noise = startTrainModel.noise,
                colorNoise = startTrainModel.colorNoise,
                partialFocus = startTrainModel.partialFocus,
                shade = startTrainModel.shade,
                hue = startTrainModel.hue,
                saturation = startTrainModel.saturation,
                maxRandomAugmentCount = startTrainModel.maxRandomAugmentCount,
                probability = startTrainModel.probability,
                borderMode = startTrainModel.borderMode
            };
        }

        private TrainRequestModel buildTrainRequestModel(StartTrainModel startTrainModel)
        {
            return new TrainRequestModel()
            {
                target_type = "venus",
                directories = startTrainModel.directories,
                train_params = new Train_params()
                {
                    gpu_id = 0,
                    iterations = startTrainModel.maxIteration,
                    network = new Network()
                    {
                        batch_size = startTrainModel.batchSize,
                        pretrain_data = startTrainModel.pretrainData,
                        width = startTrainModel.width,
                        height = startTrainModel.height,
                        channels = startTrainModel.channels
                    },
                    patchmode = new Patchmode()
                    {
                        enabled = 0,
                        width = 0,
                        height = 0
                    },
                    roi = new Roi()
                    {
                        enabled = 0,
                        x = 0,
                        y = 0,
                        width = 0,
                        height = 0
                    },
                    solver_param = new Solver_param()
                    {
                        base_learning_rate = startTrainModel.baseLearningRate,
                        gamma = startTrainModel.gamma,
                        step_count = startTrainModel.stepCount
                    },
                    augmentation = new Augmentation()
                    {
                        mirror = startTrainModel.mirror,
                        flip = startTrainModel.flip,
                        rotation90 = startTrainModel.rotation90,
                        zoom = startTrainModel.zoom,
                        tilt = startTrainModel.tilt,
                        shift = startTrainModel.shift,
                        rotation = startTrainModel.rotation,
                        contrast = startTrainModel.contrast,
                        brightness = startTrainModel.brightness,
                        smoothFiltering = startTrainModel.smoothFiltering,
                        noise = startTrainModel.noise,
                        colorNoise = startTrainModel.colorNoise,
                        partialFocus = startTrainModel.partialFocus,
                        shade = startTrainModel.shade,
                        hue = startTrainModel.hue,
                        saturation = startTrainModel.saturation,
                        maxRandomAugmentCount = startTrainModel.maxRandomAugmentCount,
                        probability = startTrainModel.probability,
                        borderMode = startTrainModel.borderMode,
                    }
                },
                class_list = new System.Collections.Generic.Dictionary<string, string>()
                {
                    { "1", "1" },
                    { "2", "2" },
                    { "3", "3" },
                }
            };
        }

        private TrainInfoModel buildTrainInfoModel(Train train, string trainStatus, TrainMetricModel trainMetric)
        {
            return new TrainInfoModel()
            {
                id = train.id,
                serverIndex = train.serverIndex,
                serverTrainId = train.serverTrainId,
                name = train.name,
                status = trainStatus,
                progress = trainMetric.current_iteration == 0 ? 0 : (trainMetric.current_iteration / trainMetric.max_iteration),
                createdAt = new ObjectId(train.id).CreationTime.ToString(),
                train_loss = trainMetric.train_loss,
                test_loss = trainMetric.test_loss,
                test_accuracy = trainMetric.test_accuracy,
                iou = trainMetric.test_accuracy2,
                iteration = trainMetric.current_iteration,
                max_iteration = trainMetric.max_iteration,
                directoryId = train.directoryId,
                trainSettingId = train.trainSettingId,
            };
        }

        private TrainSettingResponseModel buildTrainSettingResponseModel(Train train, TrainSetting trainSetting)
        {
            return new TrainSettingResponseModel()
            {
                name = train.name,
                serverIndex = train.serverIndex,

                batchSize = trainSetting.batchSize,
                pretrainData = trainSetting.pretrainData,
                width = trainSetting.width,
                height = trainSetting.height,
                channels = trainSetting.channels,
                baseLearningRate = trainSetting.baseLearningRate,
                gamma = trainSetting.gamma,
                stepCount = trainSetting.stepCount,
                maxIteration = trainSetting.maxIteration,

                mirror = trainSetting.mirror,
                flip = trainSetting.flip,
                rotation90 = trainSetting.rotation90,
                zoom = trainSetting.zoom,
                tilt = trainSetting.tilt,
                shift = trainSetting.shift,
                rotation = trainSetting.rotation,
                contrast = trainSetting.contrast,
                brightness = trainSetting.brightness,
                smoothFiltering = trainSetting.smoothFiltering,
                noise = trainSetting.noise,
                colorNoise = trainSetting.colorNoise,
                partialFocus = trainSetting.partialFocus,
                shade = trainSetting.shade,
                hue = trainSetting.hue,
                saturation = trainSetting.saturation,
                maxRandomAugmentCount = trainSetting.maxRandomAugmentCount,
                probability = trainSetting.probability,
                borderMode = trainSetting.borderMode
            };
        }
    }
}
