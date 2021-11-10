using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnet_ts_support.Models;
using MongoDB.Bson;

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
        public ActionResult<Train> StartTrain([FromBody] StartTrainModel startTrainModel)
        {
            var directory = buildDirectory(startTrainModel);
            _directoryService.Create(directory);
            var trainSetting = buildTrainSetting(startTrainModel);
            _trainSettingService.Create(trainSetting);
            // trainserver post
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
            return train;
        }

        [HttpGet("directory/{trainId}")]
        public ActionResult<Directory> GetDirectory(string trainId)
        {
            var train = _trainSerivce.Get(trainId);
            if (train == null) NotFound();
            var directory = _directoryService.Get(train.directoryId);
            if (directory == null) NotFound();
            return directory;
        }

        [HttpGet("trainSetting/{trainId}")]
        public ActionResult<TrainSetting> GetTrainSetting(string trainId)
        {
            var train = _trainSerivce.Get(trainId);
            if (train == null) NotFound();
            var trainSetting = _trainSettingService.Get(train.trainSettingId);
            if (trainSetting == null) NotFound();
            return trainSetting;
        }

        [HttpGet("dataset")]
        public ActionResult<string[]> GetDataset() => _directoryService.GetDatasets();

        [HttpGet("pretrain")]
        public ActionResult<string[]> GetPretrain() => _directoryService.GetPretrains();

        [HttpDelete("train/{id}")]
        public IActionResult DeleteTrain(string id)
        {
            // train status 검사 필요
            var train = _trainSerivce.Get(id);
            if (train == null) NotFound();
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
                        heigth = startTrainModel.height,
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


    }
}
