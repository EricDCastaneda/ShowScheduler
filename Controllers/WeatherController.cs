using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShowScheduler.Models
{
    public class WeatherController : Controller
    {
        private readonly IConfiguration _config;

        public WeatherController(IConfiguration config)
        {
            _config = config;
        }

        // GET: Weather
        public async Task<IActionResult> Current()
        {
            var weatherClient = new HttpClient();
            var OpenWeatherMapKey = _config.GetSection("API_KEYS")["OpenWeatherMapAPI"]; // To hold the API key located in secrets.json

            var stringResult = await weatherClient.GetStringAsync(
                $"http://api.openweathermap.org/data/2.5/weather?q=Dallas&appid={OpenWeatherMapKey}&units=imperial");

            var json = JObject.Parse(stringResult);
            var currentWeather = new Weather()
            {
                Condition = json["weather"][0]["main"].ToString(),
                Temp = Decimal.Round(decimal.Parse(json["main"]["temp"].ToString())),
                Wind = Decimal.Round(decimal.Parse(json["wind"]["speed"].ToString()))
            };
            return View(currentWeather);
        }
    }
}
