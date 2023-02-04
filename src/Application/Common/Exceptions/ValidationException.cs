using System.Net;

namespace Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public string Code { get; set; }
        public int Status { get; set; }

        public ValidationException(string code, string message)
            : base(message)
        {
            Code = code;
            Status = (int)HttpStatusCode.BadRequest;
        }
    }
}