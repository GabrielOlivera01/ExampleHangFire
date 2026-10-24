using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;

namespace ConsoleApp
{
    public class SimpleEnqueeExample
    {
        public void RunExample()
        {
            while (true)
            {
                try
                {
                    // 1. Connect to Redis running in Docker
                    var urlRedisInDocker = "localhost:6379";
                    var redis = ConnectionMultiplexer.Connect(urlRedisInDocker);

                    // 2. Configure Hangfire to use Redis as job storage
                    GlobalConfiguration.Configuration
                        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseRedisStorage(redis);

                    // 3. Start the Hangfire background processing server
                    using var server = new BackgroundJobServer();


                    // 4. Enqueue a background job
                    BackgroundJob.Enqueue(() => Jobs.PrintMessage());


                    Console.WriteLine("Hangfire Server started.");
                    Console.WriteLine("Press ENTER to stop the application.");

                    // 5. Keep the application running
                    Console.ReadLine();

                    redis.Dispose();
                }
                catch (StackExchange.Redis.RedisConnectionException ex)
                {
                    Console.WriteLine($"[Error][ConnectionFaild]: {ex.Message}");
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine($"[Warning][ConnectionFaild]: Verify if Redis image is running in Docker Container.");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Error][Default]: {ex.Message}");
                    Console.WriteLine();
                    Console.WriteLine();
                    throw;
                }
            }
        }
    }
}