using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Domain.Entities;
using Optern.Infrastructure.Data;
using Optern.Infrastructure.Repositories;

namespace Optern.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OpternDbContext _context;
        public IGenericRepository<PostTags> PostTags { get; private set; }
        public IGenericRepository<Room> Rooms { get; private set; }


        public UnitOfWork(OpternDbContext context) 
        {
            _context = context;
            PostTags = new  GenericRepository<PostTags>(context);  
            Rooms = new  GenericRepository<Room>(context);  
        }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();

        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
