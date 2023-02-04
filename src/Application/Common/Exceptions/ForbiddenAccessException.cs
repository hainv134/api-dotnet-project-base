using System.Net;

namespace Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public string Code { get; set; }
        public int Status { get; set; }

        public ForbiddenAccessException(string code, string message)
           : base(message)
        {
            Code = code;
            Status = (int)HttpStatusCode.Forbidden;
        }

        public static ForbiddenAccessException RestrictedAccess() => new ForbiddenAccessException("F000", "Restricted Access");
    }
}