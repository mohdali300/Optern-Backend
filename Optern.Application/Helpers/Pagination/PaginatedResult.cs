

namespace Optern.Application.Helpers.Pagination
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(List<T> data, int totalCount, int currentPage, int pageSize)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (totalCount < 0) throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count cannot be negative.");
            if (currentPage <= 0) throw new ArgumentOutOfRangeException(nameof(currentPage), "Current page must be greater than zero.");
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

            Edges = data.Select(item => new Edge<T>(item)).ToList();
            PageInfo = new PageInfo(currentPage, pageSize, totalCount);
        }

        public IReadOnlyList<Edge<T>> Edges { get; }

        public PageInfo PageInfo { get; }
    }

    public class Edge<T>
    {
        public Edge(T node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        public T Node { get; }
    }

    public class PageInfo
    {
        public PageInfo(int currentPage, int pageSize, int totalCount)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            HasPreviousPage = CurrentPage > 1;
            HasNextPage = CurrentPage < TotalPages;
        }

        public int CurrentPage { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }
    }

}
