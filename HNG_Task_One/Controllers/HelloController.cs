using HNG_Task_One.Infrastructure;
using HNG_Task_One.Model;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HNG_Task_One.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly HttpClientService _clientService;

        public HelloController(HttpClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet()]
        public async Task<IActionResult> GetInfo([FromQuery] string visitor_name)
        {
            var ipAddress = GetIpAddress(HttpContext);
            var locationInfo = await GetLocationInfoFromIp(ipAddress); //"167.71.241.136"
            var temp = await GetTemperature($"{locationInfo?.city}, {locationInfo?.country}");

            return Ok(new LocationInfo()
            {
                ClientIp = ipAddress,
                Location = $"{locationInfo?.city}, {locationInfo?.country}",
                Greetings = $"Hello, {visitor_name}!, the temperature is {temp} degrees Celsius in {locationInfo?.city}, {locationInfo?.country}"
            });
        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            return ipAddress;
        }

        private async Task<double> GetTemperature(string city)
        {
            var result = await _clientService.GetTemperature(city);

            if (result.IsSuccess)
            {
                return result.Value.GetValueOrDefault();
            }
            return 0.0;
        }

        private async Task<LocationInfoFromIp> GetLocationInfoFromIp(string ipAddress)
        {
            var result = await _clientService.GetLocationFromIp(ipAddress);

            if (result.IsSuccess) 
                return result.Value;

            return new LocationInfoFromIp();
        }
    }
}
