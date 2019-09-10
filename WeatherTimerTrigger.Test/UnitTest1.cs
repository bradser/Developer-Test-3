using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace WeatherTimerTrigger.Test
{
    public class UnitTest1
    {
        [Theory]
        [FileData("Api1.json", "Db1.json")]
        public async void Test1(string apiData, string dbData)
        {
            IHttpClientFactory factory = this.GetIHttpClientFactory(apiData);

            PoseableSoftware.Function.WeatherTimerTrigger trigger = new PoseableSoftware.Function.WeatherTimerTrigger(factory);

            TestAsyncCollector<PoseableSoftware.Function.WeatherPersistJson> weatherOut = new TestAsyncCollector<PoseableSoftware.Function.WeatherPersistJson>();

            await trigger.Run(null, weatherOut, null);

            // DecimalJsonConverter to mimic CosmoDB's float serialization
            string persistJson = JsonConvert.SerializeObject(weatherOut.Items.ToArray()[0], new DecimalJsonConverter());

            Assert.Equal(JToken.Parse(dbData).ToString(), JToken.Parse(persistJson).ToString());
        }

        private IHttpClientFactory GetIHttpClientFactory(string apiData)
        {
            var mockFactory = new Mock<IHttpClientFactory>();

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(apiData),
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("http://test.com/"),
            };

            mockFactory.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            return mockFactory.Object;
        }
    }
}
