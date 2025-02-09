

namespace Optern.Application.Helpers.Pagination
{
    public static class QueryableExtensions
    {
        public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize) where T : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Max(1, pageSize);

            var totalCount = await source.AsNoTracking().CountAsync();
            var data = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>(data, totalCount, pageNumber, pageSize);
        }
    }

}
