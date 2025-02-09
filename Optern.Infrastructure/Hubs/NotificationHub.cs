using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync(); 
        }
    }
}
