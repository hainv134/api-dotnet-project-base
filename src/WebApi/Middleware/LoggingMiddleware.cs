using Application.Common.Models;
using Domain.Global;
using Infrastructure.Services;
using Newtonsoft.Json;
using System.Text;

namespace WebApi.Middleware
{
    public class LoggingMiddleware : IMiddleware
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ICurrentUserService _currentUserService;

        public LoggingMiddleware(ILoggerFactory loggerFactory, ICurrentUserService currentUserService)
        {
            _loggerFactory = loggerFactory;
            _currentUserService = currentUserService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var _logger = _loggerFactory.CreateLogger<LoggingMiddleware>();

            context.Request.EnableBuffering();

            var requestStringData = "";
            if (context.Request.Headers.ContentType.Contains(Config.Network.CONTENT_TYPE))
            {
                requestStringData = await new StreamReader(context.Request.Body, Encoding.Default).ReadToEndAsync();
            }
            var query = context.Request.QueryString;

            var logInfo = new LoggingModel
            {
                User = _currentUserService.GetUserEmail(),
                RequestUrl = context.Request.Path,
                ContentType = context.Request.ContentType,
                RequestTime = DateTime.Now.ToLocalTime(),
                Queries = query,
                Data = JsonConvert.DeserializeObject(requestStringData),
            };

            _logger.LogInformation(JsonConvert.SerializeObject(logInfo, Formatting.Indented));

            //Rewind the request body, so it could be read again
            context.Request.Body.Position = 0;
            await next.Invoke(context);
        }
    }
}