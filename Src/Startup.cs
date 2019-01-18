using System;
using System.Net.Http;
using Hangfire;
using Hangfire.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Swashbuckle.AspNetCore.Swagger;
using TvShowsAPI.WebApi.Configuration;
using TvShowsAPI.WebApi.Context;
using TvShowsAPI.WebApi.Services;

namespace TvShowsAPI.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbConfiguration>(Configuration.GetSection("MongoDb"));
            services.Configure<ExternalApiConfiguration>(Configuration.GetSection("ExternalApi"));

            services.AddTransient<IShowRepository, ShowRepository>();
            services.AddTransient<IScrapperService, ScrapperService>();
            services.AddTransient<IShowContext, ShowContext>();
            services.AddMvc();

            //Register httpclient factory for ExternalApiService
            services.AddHttpClient<IExternalApiService, ExternalApiService>(client =>
            {
                var config = Configuration.GetSection("ExternalApi").Get<ExternalApiConfiguration>();
                client.BaseAddress = new Uri(config.Uri);
            })
            .SetHandlerLifetime(TimeSpan.FromHours(2)) //set handler lifetime to 2 hours which is the average time of scraping routine to finish
            .AddPolicyHandler(GetRateLimitRetryPolicy()); //Add retry policy to retry in case of error 429 (rate limit)


            //Configure swagger generator
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "TV Shows API", Version = "v1" });
            });

            //Configure integration with hangfire and connect it to mongo db
            services.AddHangfire(c =>
            {
                var mongoDbConfig = Configuration.GetSection("MongoDb").Get<MongoDbConfiguration>();
                c.UseMongoStorage(mongoDbConfig.ConnectionString, mongoDbConfig.HangfireCollectionName);
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions { WorkerCount = 1 });
            app.UseHangfireDashboard();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TV Shows API V1");
            });

            app.UseMvc();
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRateLimitRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
