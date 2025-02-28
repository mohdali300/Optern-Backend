using Task = System.Threading.Tasks.Task;

namespace Optern.Infrastructure.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(string id);
         Task<T> GetByIdWithIncludeAsync(string id, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByExpressionAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetQueryable(
           Expression<Func<T, bool>>? filter = null,
           string? includeProperties = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<IEnumerable<T>> GetAllByExpressionAsync(Expression<Func<T, bool>> predicate);

        public Task<IEnumerable<T>> GetAllByExpressionAsync(
   Expression<Func<T, bool>> predicate,
   Func<IQueryable<T>, IQueryable<T>> include = null);

        Task<IEnumerable<T>> GetAllAsync();
        Task SaveChangesAsync();
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(ICollection<T> entities);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>>? expression = default);

        IDbContextTransaction BeginTransaction();
        void Commit();
        void RollBack();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
