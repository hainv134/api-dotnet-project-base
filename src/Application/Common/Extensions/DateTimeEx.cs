using Domain.Global;

namespace Application.Common.Extensions
{
    public static class DateTimeEx
    {
        public static long ToUNIXTimeMiniseconds(this DateTime? dateTime)
        {
            if (dateTime == null) return 0;

            return new DateTimeOffset(dateTime.Value).ToUnixTimeMilliseconds();
        }

        public static long ToUNIXTimeMiniseconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// Convert to default date string (dd/MM/yyyy HH:mm:ss)
        /// </summary>
        /// <returns></returns>
        public static string ToDefaultDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(Config.DateTimeFormat.DEFAULT);
        }

        /// <summary>
        /// Convert to only number date string (ddMMyyyyHHmmss)
        /// </summary>
        /// <returns></returns>
        public static string ToOnlyNumberDateFormat(this DateTime dateTime)
        {
            return dateTime.ToString(Config.DateTimeFormat.NUMBER_ONLY);
        }
    }
}