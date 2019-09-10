using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace PoseableSoftware.Function
{
    public class WeatherQueryHttpTrigger
    {
        private readonly Uri _collectionUri;

        private readonly string _defaultHours = "12";

        public WeatherQueryHttpTrigger() {
            String databaseName = System.Environment.GetEnvironmentVariable("DatabaseName");

            String collectionName = System.Environment.GetEnvironmentVariable("CollectionName");

            _collectionUri = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
        }

        [FunctionName("WeatherQueryHttpTrigger")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%DatabaseName%",
                collectionName: "%CollectionName%",
                ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log)
        {
            string hoursString = getHoursString(req);

            int hours;
            if (!Int32.TryParse(hoursString, out hours))
            {
                return new BadRequestObjectResult("misformatted hours");
            }

            IDocumentQuery<WeatherPersistJson> query = getQuery(client, hours);

            StringBuilder results = await getResults(query);

            return (ActionResult)new OkObjectResult(results.ToString());
        }

        private string getHoursString(HttpRequest req)
        {
            string hoursString = req.Query["hours"];
            if (hoursString == null)
            {
                hoursString = this._defaultHours;
            }

            return hoursString;
        }

        private IDocumentQuery<WeatherPersistJson> getQuery(DocumentClient client, int hours)
        {
            DateTime then = DateTime.UtcNow - new TimeSpan(hours, 0, 0);

            // TODO: See how much performance impact by using EnableCrossPartitionQuery
            return client.CreateDocumentQuery<WeatherPersistJson>(this._collectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(wpj => wpj.DateTime.DataCalculationUtc > then)
                .AsDocumentQuery();
        }
        
        private async Task<StringBuilder> getResults(IDocumentQuery<WeatherPersistJson> query)
        {
            StringBuilder results = new StringBuilder();

            while (query.HasMoreResults)
            {
                foreach (WeatherPersistJson result in await query.ExecuteNextAsync())
                {
                    results.Append(result.GetPrettyPrint());
                    
                    results.AppendLine();
                }
            }

            return results;
        }
    }
}
