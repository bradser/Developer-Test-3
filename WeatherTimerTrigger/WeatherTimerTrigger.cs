using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace PoseableSoftware.Function
{
    public class WeatherTimerTrigger
    {
        private readonly HttpClient _client;

        public WeatherTimerTrigger(IHttpClientFactory httpClientFactory)
        {
            this._client = httpClientFactory.CreateClient(Startup.WeatherHttpClientName);
        }

        [FunctionName("WeatherTimerTrigger")]         
        public async Task Run(
            [TimerTrigger("0 */5 * * * *", RunOnStartup = true)]TimerInfo myTimer, 
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: "%CollectionName%",
                CreateIfNotExists = true,
                PartitionKey = "/location/id",
                CollectionThroughput = 400,
                ConnectionStringSetting = "CosmosDBConnection")] IAsyncCollector<WeatherPersistJson> weatherOut,
            ILogger log)
        {
            log.LogInformation("WeatherTimerTrigger fired.");

            try {
                WeatherHttpJson currentWeather = await this.getWeatherJson();

                WeatherPersistJson convertedWeather = new WeatherPersistJson(currentWeather);

                await weatherOut.AddAsync(convertedWeather);
            } catch (HttpRequestException ex) {
                log.LogError(ex, $"Failed HTTP request: {ex.ToString()}");
            }
        }

        private async Task<WeatherHttpJson> getWeatherJson() {
            var responseContent = await this._client.GetStringAsync("");

            return (WeatherHttpJson) JsonConvert.DeserializeObject<WeatherHttpJson>(responseContent);
        }
    }
}
