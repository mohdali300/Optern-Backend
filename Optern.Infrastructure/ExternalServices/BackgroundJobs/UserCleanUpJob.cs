using Hangfire;
using Optern.Infrastructure.ExternalServices.UserCleanUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
