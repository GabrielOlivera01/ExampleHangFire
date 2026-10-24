using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Redis;

namespace ConsoleApp
{
    public class SimpleEnqueeWithDashboardExample
    {
        public async Task RunExample(string[] args)
        {
            var urlRedisInDocker = "localhost:6379";
            const string BASE_URL = "http://localhost:5000";
            const string ENDPOINT_DASHBOARD = "/hangfire";
            
            var builder = WebApplication.CreateBuilder(args);

            // Configure the dashboard HTTP address.
            builder.WebHost.UseUrls(BASE_URL);

            // Connect to Redis running in Docker.
            var redis = await ConnectionMultiplexer.ConnectAsync(urlRedisInDocker);

            // Configure Hangfire to use Redis.
            builder.Services.AddHangfire(configuration =>
            {
                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseRedisStorage(redis);
            });

            // Register and start the Hangfire background server.
            builder.Services.AddHangfireServer();

            var app = builder.Build();

            // Expose the Hangfire Dashboard.
            app.UseHangfireDashboard(ENDPOINT_DASHBOARD);


            // Create a test background job.
            BackgroundJob.Enqueue(() => Jobs.PrintMessage());


            // Start the HTTP server
            Console.WriteLine($"Hangfire Dashboard: {BASE_URL}{ENDPOINT_DASHBOARD}");

            await app.RunAsync();
        }
    }
}