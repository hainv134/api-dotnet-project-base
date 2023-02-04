using Application.Common.Exceptions;
using Domain.Global;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Common.Extensions
{
    public static class StringEx
    {
        public static string GenerateSlug(this string str)
        {
            str = Regex.Replace(str, @"[^a-zA-Z0-9-\s]", "");
            str = Regex.Replace(str, @"[\s+]", " ");
            str = str.Substring(0, str.Length <= 15 ? str.Length : 15).Trim();
            str = Regex.Replace(str, @"\s", "-");

            return str;
        }

        public static string ToStringSafe(this string? str)
        {
            return str == null ? "" : str.Trim();
        }

        public static DateTime? ParseToDateTime(this string str, string? format = Config.DateTimeFormat.DEFAULT)
        {
            if (DateTime.TryParseExact(str, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }

            return null;
        }
    }
}