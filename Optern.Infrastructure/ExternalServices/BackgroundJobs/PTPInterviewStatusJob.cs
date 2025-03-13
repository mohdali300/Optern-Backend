using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalServices.BackgroundJobs
{
    public class PTPInterviewStatusJob
    {
        public void InterviewStatus()
        {
            RecurringJob.AddOrUpdate<PTPInterviewService>(
                "ChangeInterviewStatus",
                service => service.ChangeInterviewStatus(),
                Cron.Daily()
                );
        }
    }
}
