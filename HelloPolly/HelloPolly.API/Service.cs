using Polly;
using Polly.Registry;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloPolly.API
{
    public class Service : IService
    {
        private readonly IRepository _repository;
        private readonly IReadOnlyPolicyRegistry<string> _registry;

        public Service(
            IRepository repository,
            IReadOnlyPolicyRegistry<string> registry
            )
        {
            _repository = repository;
            _registry = registry;
        }

        public async Task<string> Get()
        {
            var fallback = Policy<string>
                        .Handle<Exception>()
                        .FallbackAsync(async (ct) =>
                        {
                            return await Task.FromResult("Result from fallback");
                        }, (x) =>
                        {
                            Console.WriteLine($"onFallBack: {x.Exception.Message}");
                            return Task.FromResult(x.Result);
                        });

            // TimeoutStrategy.Optimistic (default)
            return await fallback.WrapAsync(_registry.Get<IAsyncPolicy>("defaultPolicy"))
                                .ExecuteAsync(async ct => await _repository.GetData(ct), CancellationToken.None);

            // TimeoutStrategy.Pessimistic
            //return await fallback.WrapAsync(_registry.Get<IAsyncPolicy>("defaultPolicy"))
            //                    .ExecuteAsync(async () => await _repository.GetData());
        }
    }
}
