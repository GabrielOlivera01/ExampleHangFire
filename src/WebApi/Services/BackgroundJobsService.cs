using Hangfire;
using WebApi.Services.Interfaces;

namespace WebApi.Services
{
    public class BackgroundJobsService : IBackgroundJobsService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public BackgroundJobsService(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }


        // Example 1: Enqueued job
        public string EnqueedExample()
        {
            var queue = "main-queue";

            return _backgroundJobClient.Enqueue(
                queue,
                () => ExecuteExample());
        }

        // Example 2: Scheduled job
        public string ScheduledExample()
        {
            var queue = "scheduled-queue";

            return _backgroundJobClient.Schedule(
                queue,
                () => ExecuteExample(),
                TimeSpan.FromMinutes(1));
        }

        // Example 3: Recurring job
        public void RecurringJobExample()
        {
            var queue = "recurring-queue";

#pragma warning disable CS0618
            RecurringJob.AddOrUpdate(
                "example-recurring-job",
                () => ExecuteExample(),
                Cron.Minutely(),
                new RecurringJobOptions
                {
                    QueueName = queue
                });
#pragma warning restore CS0618
        }

        // Method executed by Hangfire
        public void ExecuteExample()
        {
            Console.WriteLine(
                $"Hangfire job executed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }
    }
}