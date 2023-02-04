using Application.Common.Exceptions;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace WebApi.Filters
{
    public class ApiExceptionFilterHandler : ExceptionFilterAttribute
    {
        private Dictionary<Type, Action<ExceptionContext, object?>> _exceptionHandler { get; set; }

        public ApiExceptionFilterHandler()
        {
            _exceptionHandler = new Dictionary<Type, Action<ExceptionContext, object?>>
            {
                { typeof(BussinessException), HandleBussinessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(ValidationException), HandleValidationException },
                { typeof(AuthException), HandleAuthException },
            };
        }

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            HandleException(context);

            return base.OnExceptionAsync(context);
        }

        public async void HandleException(ExceptionContext context)
        {
            context.HttpContext.Request.EnableBuffering();
            var data = await new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8).ReadToEndAsync();
            var query = context.HttpContext.Request.QueryString;
            context.HttpContext.Request.Body.Position = 0;

            var exceptionType = context.Exception.GetType();
            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
            }
            else if (_exceptionHandler.ContainsKey(exceptionType))
            {
                _exceptionHandler[exceptionType].Invoke(context, !string.IsNullOrEmpty(data) ? data : query);
            }
            else
            {
                HandleUnknownException(context);
            }
        }

        public void HandleBussinessException(ExceptionContext context, object? data = null)
        {
            var exception = (BussinessException)context.Exception;

            context.Result = new ObjectResult(ApiResponse.BussinessFailure(exception, data))
            {
                StatusCode = StatusCodes.Status200OK
            };
            context.ExceptionHandled = true;
        }

        public void HandleForbiddenAccessException(ExceptionContext context, object? data = null)
        {
            var exception = (ForbiddenAccessException)context.Exception;

            context.Result = new ObjectResult(ApiResponse.ForbiddenFailure(exception, data))
            {
                StatusCode = StatusCodes.Status200OK
            };

            context.ExceptionHandled = true;
        }

        public void HandleNotFoundException(ExceptionContext context, object? data = null)
        {
            var exception = (NotFoundException)context.Exception;

            context.Result = new ObjectResult(ApiResponse.NotFoundFailure(exception, data))
            {
                StatusCode = StatusCodes.Status200OK
            };

            context.ExceptionHandled = true;
        }

        public void HandleAuthException(ExceptionContext context, object? data = null)
        {
            var exception = (AuthException)context.Exception;

            context.Result = new ObjectResult(ApiResponse.AuthFailure(exception, data))
            {
                StatusCode = StatusCodes.Status200OK
            };

            context.ExceptionHandled = true;
        }

        public void HandleValidationException(ExceptionContext context, object? data = null)
        {
            context.ExceptionHandled = true;
        }

        public void HandleInvalidModelStateException(ExceptionContext context, object? data = null)
        {
            context.ExceptionHandled = true;
        }

        public void HandleUnknownException(ExceptionContext context, object? data = null)
        {
            var exception = context.Exception;

            context.Result = new ObjectResult(
                ApiResponse.Failure((int)StatusCodes.Status400BadRequest, exception.Message, data))
            {
                StatusCode = StatusCodes.Status200OK
            };

            context.ExceptionHandled = true;
        }
    }
}