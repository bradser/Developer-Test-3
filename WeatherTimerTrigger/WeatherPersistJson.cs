using System.Linq;
using Newtonsoft.Json;
using Unidevel.OpenWeather;

namespace PoseableSoftware.Function
{
    public class Description
    {
        [JsonProperty("id")]
        public WeatherCode Id { get; set; }

        [JsonProperty("mainDescription")]
        public string MainDescription { get; set; }

        [JsonProperty("longDescription")]
        public string LongDescription { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class Coordinates
    {
        [JsonProperty("latitudeDeg")]
        public float LatitudeDeg { get; set; }

        [JsonProperty("longitudeDeg")]
        public float LongitudeDeg { get; set; }
    }

    public class Internal
    {
        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("cod")]
        public int Cod { get; set; }

        [JsonProperty("system")]
        public WeatherPersistSystem System { get; set; }
    }

    public class Location
    {
        [JsonProperty("country")]
        public string Country;

        [JsonProperty("id")]
        public int Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }
    }

    public class Precipitation
    {
        [JsonProperty("rain")]
        public PrecipitationValues Rain;

        [JsonProperty("snow")]
        public PrecipitationValues Snow;
    }

    public class PrecipitationValues
    {
        [JsonProperty("oneHourMm")]
        public float OneHourMm { get; set; }

        [JsonProperty("threeHourMm")]
        public float ThreeHourMm { get; set; }
    }

    public class Temperature
    {
        [JsonProperty("currentC")]
        public float CurrentC { get; set; }

        [JsonProperty("minimumC")]
        public float MinimumC { get; set; }

        [JsonProperty("maximumC")]
        public float MaximumC { get; set; }
    }

    public class WeatherPersistSystem
    {
        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("message")]
        public float Message { get; set; }
    }

    public class WeatherDateTime
    {
        [JsonProperty("dataCalculationUtc")]
        public System.DateTime DataCalculationUtc { get; set; }

        [JsonProperty("timezoneShiftSeconds")]
        public int TimezoneShiftSeconds { get; set; }

        [JsonProperty("sunriseUtc")]
        public System.DateTime SunriseUtc;

        [JsonProperty("sunsetUtc")]
        public System.DateTime SunsetUtc;
    }

    public class Wind
    {
        [JsonProperty("speedKmph")]
        public float SpeedKmph { get; set; }

        [JsonProperty("directionDeg")]
        public float DirectionDeg { get; set; }
    }

    public class WeatherPersistJson
    {
        public WeatherPersistJson() {}
        
        public WeatherPersistJson(WeatherHttpJson wj)
        {
            this.CloudsPercentage = wj.Clouds.Percentage;

            this.DateTime = new WeatherDateTime()
            {
                DataCalculationUtc = wj.DateTimeUtc,
                TimezoneShiftSeconds = wj.TimezoneShiftSeconds,
                SunriseUtc = wj.Sys.SunriseUtc,
                SunsetUtc = wj.Sys.SunsetUtc
            };

            this.Descriptions = wj.Weather.Select(weather => new Description
            {
                Id = weather.Id,
                MainDescription = weather.Main,
                LongDescription = weather.Description,
                Icon = weather.Icon
            }).ToArray();

            this.HumidityPercentage = wj.Main.HumidityPerc;

            this.Internal = new Internal()
            {
                Base = wj.Base,
                Cod = wj.Cod,
                System = new WeatherPersistSystem()
                {
                    Type = wj.Sys.Type,
                    Id = wj.Sys.Id,
                    Message = wj.Sys.Message
                }
            };

            this.Location = new Location()
            {
                Id = wj.Id,
                Name = wj.Name,
                Country = wj.Sys.Country,
                Coordinates = new Coordinates
                {
                    LatitudeDeg = wj.Coordinates.LatitudeDeg,
                    LongitudeDeg = wj.Coordinates.LongitudeDeg
                }
            };

            this.Precipitation = new Precipitation();

            if (wj.Rain != null)
            {
                this.Precipitation.Rain = new PrecipitationValues
                {
                    OneHourMm = wj.Rain.OneHourMm,

                    ThreeHourMm = wj.Rain.ThreeHourMm
                };
            }

            if (wj.Snow != null)
            {
                this.Precipitation.Snow = new PrecipitationValues
                {
                    OneHourMm = wj.Snow.OneHourMm,

                    ThreeHourMm = wj.Snow.ThreeHourMm
                };
            }

            this.PressurehPa = wj.Main.PressurehPa;

            this.Temperature = new Temperature()
            {
                CurrentC = wj.Main.TempC,
                MinimumC = wj.Main.TempMinC,
                MaximumC = wj.Main.TempMaxC
            };

            this.Visibility = wj.Visibility;

            this.Wind = new Wind { SpeedKmph = wj.Wind.SpeedKmph, DirectionDeg = wj.Wind.DirectionDeg };
        }

        public string GetPrettyPrint() {
            string output = $"{this.Location.Name}, {this.Location.Country}, {this.DateTime.DataCalculationUtc.ToShortDateString()}" +
                $" {this.DateTime.DataCalculationUtc.ToShortTimeString()} - {this.GetPrettyPrintDescription()}" +
                $". Temperature is {this.Temperature.CurrentC} degrees C.";

            return output;
        }

        private string GetPrettyPrintDescription() {
            return this.Descriptions[0].LongDescription.ElementAt(0).ToString().ToUpper() +
                    this.Descriptions[0].LongDescription.Substring(1);
        }

        [JsonProperty("cloudsPercentage")]
        public float CloudsPercentage { get; set; }

        [JsonProperty("dateTime")]
        public WeatherDateTime DateTime { get; set; }

        [JsonProperty("descriptions")]
        public Description[] Descriptions { get; set; }

        [JsonProperty("humidityPercentage")]
        public float HumidityPercentage { get; set; }

        [JsonProperty("internal")]
        public Internal Internal { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("precipitation")]
        public Precipitation Precipitation { get; set; }

        [JsonProperty("pressurehPa")]
        public float PressurehPa { get; set; }

        [JsonProperty("temperature")]
        public Temperature Temperature { get; set; }

        [JsonProperty("visibility")]
        public float Visibility { get; set; }

        [JsonProperty("wind")]
        public Wind Wind { get; set; }
    }
}