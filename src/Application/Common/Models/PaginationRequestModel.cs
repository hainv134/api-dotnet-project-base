using Domain.Global;
using System.ComponentModel;

namespace Application.Common.Models
{
    public class PaginationRequestModel
    {
        [DefaultValue(Config.Pagination.PAGE_NUMBER)]
        public int PageNumber { get; set; }

        [DefaultValue(Config.Pagination.PAGE_SIZE)]
        public int PageSize { get; set; }
    }
}