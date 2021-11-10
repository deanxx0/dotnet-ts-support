using dotnet_ts_support.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace dotnet_ts_support.Services
{    
    public class TrainService
    {
        private readonly IMongoCollection<Train> _trains;
        private readonly HttpClient _httpClient;

        public TrainService(IDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DBName);
            _trains = db.GetCollection<Train>("trains");

            _httpClient = new HttpClient();
        }

        public Train Create(Train train)
        {
            _trains.InsertOne(train);
            return train;
        }

        public List<Train> Get() => _trains.Find(train => true).ToList();

        public Train Get(string id) => _trains.Find(train => train.id == id).FirstOrDefault();

        public void Update(string id, Train trainIn) => _trains.ReplaceOne(train => train.id == id, trainIn);

        public void Remove(string id) => _trains.DeleteOne(train => train.id == id);

        // return model?
        public async Task<TrainResponseModel> PostTrainToServer(string trainServerUri, TrainRequestModel trainRequestModel)
        {
            try
            {
                using(var response = await _httpClient.PostAsJsonAsync($"http://{trainServerUri}/trains", trainRequestModel))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var jsonDoc = JsonDocument.Parse(body).RootElement;
                        ApiResponseModel apiResponse = JsonSerializer.Deserialize<ApiResponseModel>(jsonDoc.GetRawText());

                        var resultBody = jsonDoc.GetProperty("result").GetRawText();
                        var resultJsonDoc = JsonDocument.Parse(resultBody).RootElement;
                        TrainResponseModel trainResponse = JsonSerializer.Deserialize<TrainResponseModel>(resultJsonDoc.GetRawText());
                        return trainResponse;
                    }
                }
            }
            catch(HttpRequestException e)
            {
                Console.WriteLine($"error! {e.Message}");
                return null;
            }

            return null;
            //using (var response = await _httpClient.PostAsJsonAsync($"http://{trainServerUri}/trains", trainRequestModel))
            //{
            //    if (HttpStatusCode.OK == response.StatusCode)
            //    {
            //        string body = await response.Content.ReadAsStringAsync();

            //        var doc = JsonDocument.Parse(body);
            //        var root = doc.RootElement;
            //        var successs = root.GetProperty("success").GetBoolean();
            //        if (successs)
            //        {
            //            var result = root.GetProperty("result");
            //            var resultDoc = JsonDocument.Parse(result.GetRawText()).RootElement;
            //            var trainResponseModel = JsonSerializer.Deserialize<TrainResponseModel>(resultDoc);

            //            //train model = new train()
            //            //{
            //            //    Id = result.GetProperty("id").GetString(),
            //            //    status = result.GetProperty("status").GetString(),
            //            //    error_message = result.GetProperty("error_message").GetString(),
            //            //    created_time = result.GetProperty("created_time").GetDateTime(),
            //            //    remain_time_ms = (double)result.GetProperty("remain_time_ms").GetDecimal(),
            //            //    process_time_ms = (double)result.GetProperty("process_time_ms").GetDecimal(),
            //            //};
            //        }
            //    }
            //}
        }
    }
}
