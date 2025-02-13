

namespace Optern.Domain.Entities
{
    public class BookMarkedTask
    {
        public int Id { get; set; }

        //Foreign Keys
        public string UserId { get; set; }
        public int TaskId { get; set; }

        // Navigation Properties
        public virtual ApplicationUser User { get; set; }
        public virtual Task Task { get; set; }
    }
}
