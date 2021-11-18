using dotnet_ts_support.Models;
using MongoDB.Bson;
using MongoDB.Driver;
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

        public Task<long> GetTotalCount() => _trains.CountDocumentsAsync(new BsonDocument());

        public Train[] GetTrainPage(int pageNo, int perPage)
        {
            var trains = _trains.Find(train => true).SortByDescending(train => train.id).Limit(perPage).Skip(perPage * (pageNo - 1)).ToList();
            return trains.ToArray();
            //var train = _trains.Find(train => true).FirstOrDefaultAsync().Result;
            //var objId = new ObjectId(train.id);
            //var time = objId.CreationTime;
            //Console.WriteLine(time);
            //foreach(Train train in trains)
            //{
            //    Console.WriteLine($"{train.id} / {train.name}");
            //}
        }

        public async Task<ResourceModel> GetResources(string uri)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"http://{uri}/resources"))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var jsonDoc = JsonDocument.Parse(body).RootElement;
                        var resultBody = jsonDoc.GetProperty("result").GetRawText();
                        var resultJsonDoc = JsonDocument.Parse(resultBody).RootElement;
                        ResourceModel resourceResponse = JsonSerializer.Deserialize<ResourceModel>(resultJsonDoc.GetRawText());
                        return resourceResponse;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return null;
            }
            return null;
        }

        public void Update(string id, Train trainIn) => _trains.ReplaceOne(train => train.id == id, trainIn);

        public void Remove(string id) => _trains.DeleteOne(train => train.id == id);

        public async Task<TrainResponseModel> PostTrainToServer(string trainServerUri, TrainRequestModel trainRequestModel)
        {
            try
            {
                using (var response = await _httpClient.PostAsJsonAsync($"http://{trainServerUri}/trains", trainRequestModel))
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
                return null;
            }
            return null;
        }

        public async Task<string> GetStatusFromServer(string trainServerUri, string serverTrainId)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"http://{trainServerUri}/trains/{serverTrainId}"))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var jsonDoc = JsonDocument.Parse(body).RootElement;
                        var resultBody = jsonDoc.GetProperty("result").GetRawText();
                        var resultJsonDoc = JsonDocument.Parse(resultBody).RootElement;
                        TrainResponseModel trainResponse = JsonSerializer.Deserialize<TrainResponseModel>(resultJsonDoc.GetRawText());
                        return trainResponse.status;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return null;
            }
            return null;
        }

        public async Task<TrainMetricModel> GetMetricFromServer(string trainServerUri, string serverTrainId)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"http://{trainServerUri}/trains/{serverTrainId}/metrics/pages/0"))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var jsonDoc = JsonDocument.Parse(body).RootElement;
                        var resultBody = jsonDoc.GetProperty("result").GetRawText();
                        var resultJsonDoc = JsonDocument.Parse(resultBody).RootElement;
                        TrainMetricModel[] metricResponse = JsonSerializer.Deserialize<TrainMetricModel[]>(resultJsonDoc.GetRawText());
                        if(metricResponse.Length == 0)
                        {
                            return new TrainMetricModel()
                            {
                                train_id = serverTrainId,
                                max_iteration = 0,
                                current_iteration = 0,
                                train_loss = 0,
                                test_accuracy = 0,
                                test_loss = 0,
                                test_accuracy2 = 0
                            };
                        }
                        else
                        {
                            return new TrainMetricModel()
                            {
                                train_id = metricResponse[metricResponse.Length-1].train_id,
                                max_iteration = metricResponse[metricResponse.Length - 1].max_iteration,
                                current_iteration = metricResponse[metricResponse.Length - 1].current_iteration,
                                train_loss = metricResponse[metricResponse.Length - 1].train_loss,
                                test_accuracy = metricResponse[metricResponse.Length - 1].test_accuracy,
                                test_loss = metricResponse[metricResponse.Length - 1].test_loss,
                                test_accuracy2 = metricResponse[metricResponse.Length - 1].test_accuracy2
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return null;
            }
            return null;
        }

        public async Task<TrainMetricModel[]> GetMetricPageFromServer(string trainServerUri, string serverTrainId, int pageNo)
        {
            try
            {
                using (var response = await _httpClient.GetAsync($"http://{trainServerUri}/trains/{serverTrainId}/metrics/pages/{pageNo}"))
                {
                    if (HttpStatusCode.OK == response.StatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        var jsonDoc = JsonDocument.Parse(body).RootElement;
                        var resultBody = jsonDoc.GetProperty("result").GetRawText();
                        var resultJsonDoc = JsonDocument.Parse(resultBody).RootElement;
                        TrainMetricModel[] metricResponse = JsonSerializer.Deserialize<TrainMetricModel[]>(resultJsonDoc.GetRawText());
                        //List<TrainMetricModel> metrics = metricResponse.ToList();
                        if (metricResponse.Length == 0)
                        {
                            //metrics.Add(new TrainMetricModel()
                            //    {
                            //        train_id = serverTrainId,
                            //        max_iteration = 0,
                            //        current_iteration = 0,
                            //        train_loss = 0,
                            //        test_accuracy = 0,
                            //        test_loss = 0,
                            //        test_accuracy2 = 0
                            //    }
                            //);
                            return null;
                        }
                        else
                        {
                            return metricResponse;
                            //return metrics;
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                return null;
            }
            return null;
        }
    }
}
