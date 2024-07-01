using CSharpFunctionalExtensions;
using HNG_Task_One.Model;
using Newtonsoft.Json;

namespace HNG_Task_One.Infrastructure
{
    public class HttpClientService
    {
        private readonly HttpClient _client;
        private readonly ILogger<HttpClientService> _logger;

        private const string AppId = "8c65048ebb9e02b374b09b55574aac8a";
        private const string Token = "586cfa5523c59f";
        

        public HttpClientService(HttpClient client, ILogger<HttpClientService> logger)
        {
            _client = client;
            _logger = logger;
        }

        private static class Endpoint
        {
            public static string OpenWeatherMapUrl => $"http://api.openweathermap.org/data/2.5/weather?q={{0}}&appid={AppId}&units=metric";
            public static string IpInfoUrl => $"https://ipinfo.io/{{0}}?token={Token}";
            
        }

        public async Task<Result<double?>> GetTemperature(string city)
        {
            try
            { 
                var url = string.Format(Endpoint.OpenWeatherMapUrl, city);

                var get = await _client.GetAsync(url);
                var response = await get.Content.ReadAsStringAsync();

                if (get.IsSuccessStatusCode)
                {
                    var temperatureInfo = JsonConvert.DeserializeObject<TemperatureInfo>(response);
                    return Result.Success(temperatureInfo.main?.temp);
                }

                var temperatureFailure = JsonConvert.DeserializeObject<TemperatureFailure>(response);
                _logger.LogError(temperatureFailure.message);
                return Result.Failure<double?>(temperatureFailure.message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Result.Failure<double?>("We're having trouble reaching our partner at this moment. Please try again later");
            }
        }

        public async Task<Result<LocationInfoFromIp>> GetLocationFromIp(string ipAddress)
        {
            try
            {
                var url = string.Format(Endpoint.IpInfoUrl, ipAddress);

                var get = await _client.GetAsync(url);
                var response = await get.Content.ReadAsStringAsync();

                if (get.IsSuccessStatusCode)
                {
                    var locationInfo = JsonConvert.DeserializeObject<LocationInfoFromIp>(response);
                    return Result.Success(locationInfo);
                }

                var temperatureFailure = JsonConvert.DeserializeObject<LocationInfoFromIp>(response);
                _logger.LogError(temperatureFailure.ToString());
                return Result.Failure<LocationInfoFromIp>(temperatureFailure.country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Result.Failure<LocationInfoFromIp>("We're having trouble reaching our partner at this moment. Please try again later");
            }
        }
    }
}
