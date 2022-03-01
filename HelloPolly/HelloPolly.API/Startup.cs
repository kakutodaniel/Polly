using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Registry;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelloPolly.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IService, Service>();
            services.AddScoped<IRepository, Repository>();

            var retry = Policy
                .Handle<Exception>()
                .Or<ArgumentException>()
                // .Or<TimeoutRejectedException>()
                //.Or<TimeoutException>()
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromMilliseconds(2), (ex, timespan, context) =>
                {
                    dynamic ctx = JsonConvert.DeserializeObject(context["data"].ToString());
                    string desc = ctx.Description;
                    
                    Console.WriteLine($"Wait and Retry: {context.PolicyKey} at {context.OperationKey}");
                });

            var breaker = Policy
                .Handle<Exception>()
                .Or<ArgumentException>()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(10),
                    (ex, timespan) => 
                    {
                        Console.WriteLine($"on break: {ex.Message}");
                    },
                    () => 
                    {
                        Console.WriteLine("on reset...");
                    });

            var timeout = Policy
                .TimeoutAsync(TimeSpan.FromMilliseconds(5), onTimeoutAsync: (context, timespan, task) =>
                 {
                     Console.WriteLine($"{context.PolicyKey} at {context.OperationKey}: execution timed out after {timespan.TotalSeconds} seconds.");
                     return Task.CompletedTask;
                 });

            var policyWrap = Policy.WrapAsync(retry, breaker, timeout);

            services.AddPolicyRegistry()
                        .Add("defaultPolicy", policyWrap);

            // optional
            //registry.Add("defaultPolicy", policyWrap);
            //services.AddScoped<IReadOnlyPolicyRegistry<string>>(x => registry);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
