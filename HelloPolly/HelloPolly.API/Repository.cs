using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloPolly.API
{
    public class Repository : IRepository
    {
        public async Task<string> GetData(CancellationToken ct = default)
        {
            Console.WriteLine(StaticData.Count);

            if (StaticData.Count == 1)
            {
                StaticData.IncrementCount();
                throw new NotImplementedException();
            }

            StaticData.IncrementCount();

            if (StaticData.Count % 7 == 0)
            {
                return await Task.FromResult($"Count is {StaticData.Count}");
            }

            // TimeoutStrategy.Optimistic (default)
            await Task.Delay(TimeSpan.FromSeconds(25), ct);

            // TimeoutStrategy.Pessimistic
            //await Task.Delay(TimeSpan.FromSeconds(25));

            return "passed";
        }
    }
}
