using System;
using System.Net;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;

[assembly: FunctionsStartup(typeof(PoseableSoftware.Function.Startup))]

namespace PoseableSoftware.Function
{
    public class Startup : FunctionsStartup
    {
        public static string WeatherHttpClientName = "WeatherAPI";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient(WeatherHttpClientName, client =>
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                client.BaseAddress = this.getWeatherApiUrl();
            })
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound) // TODO: log HttpStatusCode.Unauthorized
                    .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(5, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))) // TODO: jitter?
            );
        }

        private Uri getWeatherApiUrl()
        {
            Uri weatherApiUrl;
            bool created = Uri.TryCreate(System.Environment.GetEnvironmentVariable("WeatherApiUrl"),
                                            UriKind.Absolute, out weatherApiUrl);

            if (!created)
            {
                throw new ArgumentException("WeatherApiUrl environment variable missing/misformatted");
            }

            return weatherApiUrl;
        }
    }
}