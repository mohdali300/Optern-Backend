using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Hubs
{
    public class PTPInterviewHub(IPTPInterviewService pTPInterviewService):Hub
    {
        private readonly IPTPInterviewService _pTPInterviewService = pTPInterviewService;

        public async Task JoinInterviewSession()
        {
            throw new NotImplementedException();
        }
    }
}
