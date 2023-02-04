using Domain.Global;

namespace Application.Common.Models
{
    public abstract class BaseRequestModel
    {
        public SortRequestModel[]? Sort { get; set; }

        public PaginationRequestModel Pagination { get; set; } = new PaginationRequestModel
        {
            PageNumber = Config.Pagination.PAGE_NUMBER,
            PageSize = Config.Pagination.PAGE_SIZE,
        };
    }
}