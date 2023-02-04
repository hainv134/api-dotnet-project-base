using Application.Common.Exceptions;
using System.Net;

namespace Application.Common.Models
{
    public class ApiResponse
    {
        public ApiResponse(bool succeeded, int status, string code, string messages, object? data = null)
        {
            Succeeded = succeeded;
            Status = status;
            Code = code;
            Messages = messages;
            Data = data;
        }

        public bool Succeeded { get; set; }
        public int Status { get; set; }
        public string? Code { get; set; }
        public string? Messages { get; set; }
        public object? Data { get; set; }

        public static ApiResponse Success(object? data = null)
        {
            return new ApiResponse(true, (int)HttpStatusCode.OK, string.Empty, "Successfull", data);
        }

        public static ApiResponse Failure(int statusCode, string messages, object? errors = null)
        {
            return new ApiResponse(false, statusCode, string.Empty, messages, errors);
        }

        public static ApiResponse AuthFailure(AuthException ex, object? data = null)
        {
            return new ApiResponse(false, ex.Status, ex.Code, ex.Message, data);
        }

        public static ApiResponse BussinessFailure(BussinessException ex, object? data = null)
        {
            return new ApiResponse(false, ex.Status, ex.Code, ex.Message, data);
        }

        public static ApiResponse NotFoundFailure(NotFoundException ex, object? data = null)
        {
            return new ApiResponse(false, ex.Status, ex.Code, ex.Message, data);
        }

        public static ApiResponse ForbiddenFailure(ForbiddenAccessException ex, object? data = null)
        {
            return new ApiResponse(false, ex.Status, ex.Code, ex.Message, data);
        }
    }
}