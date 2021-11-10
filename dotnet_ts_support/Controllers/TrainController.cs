using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using dotnet_ts_support.Models;
using MongoDB.Bson;

namespace dotnet_ts_support.Controllers
{
    [Route("train")]
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

        [HttpPost]
        public Train StartTrain([FromBody] StartTrainModel startTrainModel)
        {
            var directory = buildDirectory(startTrainModel);
            _directoryService.Create(directory);
            var trainSetting = buildTrainSetting(startTrainModel);
            _trainSettingService.Create(trainSetting);
            string serverTrainId = "server train id"; // trainserver post
            var train = buildTrain(
                startTrainModel.serverIndex, 
                startTrainModel.name, 
                serverTrainId, 
                directory.id, 
                trainSetting.id
                );
            _trainSerivce.Create(train);
            return train;
        }

        [AllowAnonymous]
        [HttpPost("trainServerInfo")]
        public TrainServerInfo CreateTrainServerInfo([FromBody] TrainServerInfo trainServerInfo)
        {
            _trainServerInfoService.Create(trainServerInfo);
            return trainServerInfo;
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



    }
}
