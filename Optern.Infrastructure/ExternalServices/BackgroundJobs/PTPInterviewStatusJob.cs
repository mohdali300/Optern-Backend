
namespace Optern.Infrastructure.ExternalServices.BackgroundJobs
{
    public class PTPInterviewStatusJob
    {
        public void InterviewStatus()
        {
            RecurringJob.AddOrUpdate<PTPInterviewService>(
                "ChangeInterviewStatus",
                service => service.HandleInterviewStatus(),
                Cron.Daily()
                );
        }
    }
}
