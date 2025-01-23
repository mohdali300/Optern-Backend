using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Optern.Infrastructure.Data;

namespace Optern.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly OpternDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(OpternDbContext context)
        {
            _dbContext = context;
            _dbSet = context.Set<T>();
        }


        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? expression = default)
        {
            if (expression is null)
            {
                return await _dbSet.CountAsync();
            }
                return await _dbSet.CountAsync(expression);
        }
        public virtual async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
                return await _dbSet.ToListAsync();

        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
          return  await _dbSet.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();

        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            _dbSet.UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}
