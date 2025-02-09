namespace Optern.Infrastructure.ExternalServices.UserCleanUp
{
    public class UserCleanUpService
    {
        private readonly OpternDbContext _context;

        public UserCleanUpService(OpternDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task CleanUnConfirmedUserAsync()
        {
            var threshold = DateTime.UtcNow.AddHours(-24);
            var unConfirmedUsers = await _context.Users
                .Where(u => !u.EmailConfirmed && u.CreatedAt < threshold).ToListAsync();

            if (unConfirmedUsers.Any())
            {
                _context.RemoveRange(unConfirmedUsers);
                await _context.SaveChangesAsync();
            }
        }
    }
}
