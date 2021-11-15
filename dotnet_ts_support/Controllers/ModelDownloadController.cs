using dotnet_ts_support.Models;
using dotnet_ts_support.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace dotnet_ts_support.Controllers
{
    [Route("files/modelfiles")]
    [ApiController]
    [Authorize]
    public class ModelDownloadController : ControllerBase
    {
        string _outputPath;
        private readonly TrainService _trainSerivce;

        public ModelDownloadController(IConfiguration configuration, TrainService trainService)
        {
            _outputPath = configuration.GetValue<string>("TRAIN_OUTPUT_DIR");
            _trainSerivce = trainService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Download(string id)
        {
            Train train = _trainSerivce.Get(id);
            if (train == null) return NotFound();

            //back-trainid => trainserver-trainid
            string trainserverId = train.serverTrainId;

            //back-trainName
            string trainName = train.name;        

            //backend-trainid =>  serverId

            var path = Path.Combine(_outputPath, trainserverId, "models", "model.dat");
            if (System.IO.File.Exists(path))
            {
                byte[] bytes;
                using (FileStream file = new FileStream(path: path, mode: FileMode.Open))
                {
                    try
                    {
                        bytes = new byte[file.Length];
                        await file.ReadAsync(bytes);

                        Response.Headers.Add("file-name", $"{trainName}.dat");
                        return File(bytes, "application/octet-stream");
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError);
                    }
                }
            }
            else { return NotFound(); }

        }
    }
}
