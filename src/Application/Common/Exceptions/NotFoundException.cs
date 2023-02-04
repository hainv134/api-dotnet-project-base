using System.Net;

namespace Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public string Code { get; set; }
        public int Status { get; set; }

        public NotFoundException(string code, string message)
            : base(message)
        {
            Code = code;
            Status = (int)HttpStatusCode.NotFound;
        }

        public static NotFoundException ResourceNotFound() => new NotFoundException("N000", "Resource not found");
    }
}