using Domain.Global;

namespace Application.Common.Models
{
    public class PaginatedList<T>
    {
        public PaginatedList(
            List<T> items,
            int count,
            int pageNumber = Config.Pagination.PAGE_NUMBER,
            int pageSize = Config.Pagination.PAGE_SIZE)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            Items = items;
        }

        public List<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;
    }
}