using Microsoft.EntityFrameworkCore;

namespace Optern.Infrastructure.Data
{
    public class OpternDbContext : DbContext
    {
         public OpternDbContext(DbContextOptions<OpternDbContext> options) : base(options)
		 {
		 	
		 }
    }
}