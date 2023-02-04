using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get current userid from claim
        /// </summary>
        public string GetUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Get current user name from claim
        /// </summary>
        public string GetUserName()
        {
            var claim = _httpContextAccessor.HttpContext?.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Get current user email from claim
        /// </summary>
        public string GetUserEmail()
        {
            var claim = _httpContextAccessor.HttpContext?.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Check if user have already logged
        /// </summary>
        public bool IsUserLoggedIn() => _httpContextAccessor.HttpContext?.User.Claims.Any() ?? false;

        /// <summary>
        /// Get token from Authorization Header
        /// </summary>
        public string GetToken()
        {
            var rawToken = _httpContextAccessor.HttpContext?.Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(rawToken)) return string.Empty;

            return Regex.Replace(rawToken, "^[\\w]+\\s+", "");
        }
    }

    public interface ICurrentUserService
    {
        public string GetUserId();

        public string GetUserName();

        public string GetUserEmail();

        public bool IsUserLoggedIn();

        public string GetToken();
    }
}