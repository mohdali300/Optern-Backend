using Microsoft.EntityFrameworkCore;
using Optern.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optern.Infrastructure.ExternalServices.UserCleanUp
{
    public class UserCleanUpService
    {
        private readonly OpternDbContext _context;

        public UserCleanUpService(OpternDbContext context)
        {
            _context = context;
        }

        public async Task CleanUnConfirmedUserAsync()
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
