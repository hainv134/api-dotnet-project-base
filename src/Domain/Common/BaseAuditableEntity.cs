using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        [Column(TypeName = "NVARCHAR(320)")]
        public string? CreatedBy { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? UpdatedAt { get; set; }

        [Column(TypeName = "NVARCHAR(320)")]
        public string? UpdatedBy { get; set; }
    }
}