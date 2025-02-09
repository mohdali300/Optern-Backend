namespace Optern.Infrastructure.ExternalServices.BackgroundJobs
{
    public class UserCleanUpJob
    {
        public void UserCleanUp()
        {
            RecurringJob.AddOrUpdate<UserCleanUpService>(
                "CleanUnConfirmedUsers",
                service => service.CleanUnConfirmedUserAsync(),
                Cron.Daily(0)
                );
        }
    }
}
