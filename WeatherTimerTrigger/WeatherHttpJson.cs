using Newtonsoft.Json;
using Unidevel.OpenWeather;

namespace PoseableSoftware.Function
{
    public class WeatherHttpJson : CurrentWeather
    {
        [JsonProperty("visibility")]
        public uint Visibility { get; set; }
    }
}