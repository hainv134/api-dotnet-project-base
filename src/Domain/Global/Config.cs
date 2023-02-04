using System.Text;

namespace Domain.Global
{
    public static class Config
    {
        public static class ContentType
        {
            /// <summary>
            /// Excel, ...
            /// </summary>
            public const string OctetStream = "application/octet-stream";
            public const string Json = "application/json";
        }

        public static class Network
        {
            public static Encoding ENCODING = Encoding.UTF8;
            public const string USER_AGENT = "Mozilla/5.0";
            public const string CONTENT_TYPE = ContentType.Json;
            public const string DEFAULT_AUTH_SCHEMA = "Bearer";
        }

        public static class Pagination
        {
            public const int PAGE_NUMBER = 1;
            public const int PAGE_SIZE = 10;
        }

        public static class DateTimeFormat
        {
            /// <summary>
            /// Example: 13/04/2000 23:23:00
            /// </summary>
            public const string DEFAULT = "dd/MM/yyyy HH:mm:ss";

            /// <summary>
            /// Example: 13042000232300
            /// </summary>
            public const string NUMBER_ONLY = "ddMMyyyyHHmmss";
        }
    }
}