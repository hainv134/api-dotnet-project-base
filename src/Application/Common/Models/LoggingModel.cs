using Microsoft.AspNetCore.Http;

namespace Application.Common.Models
{
    public class LoggingModel
    {
        public string? User { get; set; }
        public string? RequestUrl { get; set; }
        public string? ContentType { get; set; }
        public DateTime RequestTime { get; set; }
        public QueryString Queries { get; set; }
        public object? Data { get; set; }
    }
}