namespace WebApi.Services.Interfaces
{
    public interface IBackgroundJobsService
    {
        string EnqueedExample();
        string ScheduledExample();
        void RecurringJobExample();

        void ExecuteExample();
    }
}