using Domain.Common;
using Domain.Global;
using System.ComponentModel;

namespace Application.Common.Models
{
    public class SortRequestModel
    {
        [DefaultValue(nameof(BaseAuditableEntity.UpdatedAt))]
        public string Name { get; set; } = nameof(BaseAuditableEntity.UpdatedAt);

        [DefaultValue(nameof(EnumData.SortDir.DESC))]
        public string? Dir { get; set; } = nameof(EnumData.SortDir.DESC);
    }
}