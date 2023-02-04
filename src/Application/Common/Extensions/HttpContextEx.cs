using Application.Common.Interfaces;
using Domain.Global;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Application.Common.Extensions
{
    public static class HttpContextEx
    {
        /// <summary>
        /// Get current userid from claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetUserId(this HttpContext httpContext)
        {
            var claim = httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Get current user name from claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetUserName(this HttpContext httpContext)
        {
            var claim = httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Name);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Get current user email from claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetUserEmail(this HttpContext httpContext)
        {
            var claim = httpContext.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Get current user role from claim
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetUserRole(this HttpContext httpContext)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return claim == null ? "" : claim.Value;
        }

        /// <summary>
        /// Check if user have already logged
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static bool IsUserLoggedIn(this HttpContext httpContext)
        {
            return httpContext.User.Claims.Any();
        }

        /// <summary>
        /// Get token from Authorization Header
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetToken(this HttpContext httpContext)
        {
            var rawToken = httpContext.Request.Headers[HeaderNames.Authorization].ToString();
            if (string.IsNullOrEmpty(rawToken)) return string.Empty;

            return Regex.Replace(rawToken, "^[\\w]+\\s+", "");
        }

    }
}