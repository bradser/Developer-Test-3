using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WeatherTimerTrigger.Test
{
    // It was faster to copy this class from examples, vs. configuring it in Moq:
    // https://devxp.blogspot.com/2017/07/unit-testing-your-azure-functions-part.html

    public class TestAsyncCollector<T> : IAsyncCollector<T>
    {
        public readonly List<T> Items = new List<T>();

        public Task AddAsync(T item, CancellationToken cancellationToken = default(CancellationToken))
        {
            Items.Add(item);

            return Task.FromResult(true);
        }

        public Task FlushAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(true);
        }
    }
}