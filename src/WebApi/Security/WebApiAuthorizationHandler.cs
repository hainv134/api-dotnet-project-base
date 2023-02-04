using Application.Common.Interfaces;
using Domain.Global;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApi.Security
{
    public class WebApiAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<WebApiAuthorizationHandler> _logger;

        public WebApiAuthorizationHandler(
            ILogger<WebApiAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var httpContext = context.Resource as HttpContext;
            var pendingRequirements = context.PendingRequirements.ToList();

            // Handler User Define requirement
            // context.Succeed(pendingRequirement);
            _logger.LogInformation("Authorized Request");
            return Task.CompletedTask;
        }
    }
}