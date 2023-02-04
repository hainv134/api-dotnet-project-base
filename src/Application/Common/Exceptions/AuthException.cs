using System.Net;

namespace Application.Common.Exceptions
{
    public class AuthException : Exception
    {
        public string Code { get; set; }
        public int Status { get; set; }

        public AuthException(string code, string message)
            : base(message)
        {
            Code = code;
            Status = (int)HttpStatusCode.Unauthorized;
        }
    }
}