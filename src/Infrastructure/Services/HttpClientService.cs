using Domain.Global;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Infrastructure.Services
{
    public class HttpClientService : HttpClient
    {
        private readonly ILogger<HttpClientService> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly Encoding _encoding = Config.Network.ENCODING;
        private readonly string _contentType = Config.Network.CONTENT_TYPE;
        private readonly string _authSchema = Config.Network.DEFAULT_AUTH_SCHEMA;

        public HttpClientService(
            ILogger<HttpClientService> logger,
            ICurrentUserService currentUserService)
        {
            // Default request headler
            // Only accept json string request body data
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_contentType));
            DefaultRequestHeaders.Add("accept", "text/plain");

            _logger = logger;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Send an POST request to specific url
        /// </summary>
        public async Task<HttpResponseMessage> SendAsync(string uri, string token = "", object? data = null)
        {
            return await SendRequestAsync(HttpMethod.Post, uri, token, data);
        }

        /// <summary>
        /// Send a GET request to specific url
        /// </summary>
        public async Task<HttpResponseMessage> GetAsync(string uri, string token = "")
        {
            return await SendRequestAsync(HttpMethod.Get, uri, token);
        }

        /// <summary>
        /// Send a DELETE request to specific url
        /// </summary>
        public async Task<HttpResponseMessage> DeleteAsync(string uri, string token = "")
        {
            return await SendRequestAsync(HttpMethod.Delete, uri, token);
        }

        /// <summary>
        /// Send a PUT request to specific url
        /// </summary>
        public async Task<HttpResponseMessage> PutAsync(string uri, string token = "", object? data = null)
        {
            return await SendRequestAsync(HttpMethod.Put, uri, token, data);
        }

        /// <summary>
        /// Function to send a http request to external api
        /// </summary>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string uri, string token = "", object? data = null)
        {
            _logger.LogInformation($"Sending an API {method.Method} request to {uri}");

            // Create http request
            var requestMessage = new HttpRequestMessage();
            requestMessage.RequestUri = new Uri(uri);
            switch (method.Method.ToUpper())
            {
                case "GET":
                    requestMessage.Method = HttpMethod.Get;
                    break;

                case "POST":
                    requestMessage.Method = HttpMethod.Post;
                    break;

                case "PUT":
                    requestMessage.Method = HttpMethod.Put;
                    break;

                case "DELETE":
                    requestMessage.Method = HttpMethod.Delete;
                    break;

                default:
                    break;
            }
            if (string.IsNullOrEmpty(token))
            {
                token = _currentUserService.GetToken();
            }
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue(_authSchema, token);

            if (data != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(data), _encoding, _contentType);
            }
            // Receieve http response
            var responseMessage = await SendAsync(requestMessage);
            responseMessage.LogResponse(_logger);
            return responseMessage;
        }
    }

    public static class HttpResponseMessageEx
    {
        public static async Task<string> GetAPIResponseData(this HttpResponseMessage? response)
        {
            if (response == null) return "";

            string responseText = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<BaseResponseModel>(responseText);
            if (apiResponse == null)
            {
                return "";
            }

            if (apiResponse.Data == null || !apiResponse.Succeeded)
            {
                return "";
            }

            return apiResponse.Data.ToString() ?? "";
        }

        public static void LogResponse(this HttpResponseMessage? response, ILogger logger)
        {
            if (response == null) return;
            var loggingModel = new ResponseLoggingModel
            {
                Success = response.IsSuccessStatusCode,
                RequestUri = response.RequestMessage?.RequestUri?.AbsoluteUri,
                ResponseTime = DateTime.Now.ToLocalTime(),
            };

            var responseContentType = response.Content.Headers.ContentType;
            if (responseContentType != null && responseContentType.MediaType != null && responseContentType.MediaType.Contains(Config.Network.CONTENT_TYPE))
            {
                loggingModel.ResponseData = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }
            // Check response is success or not
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation(JsonConvert.SerializeObject(loggingModel, Formatting.Indented));
            }
            else
            {
                logger.LogError(JsonConvert.SerializeObject(loggingModel, Formatting.Indented));
            }
        }

        public static string GetResponseData(this HttpResponseMessage? response)
        {
            if (response == null) return "";
            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }

    public class BaseResponseModel
    {
        public bool Succeeded { get; set; }
        public int Status { get; set; }
        public string? Code { get; set; }
        public string? Messages { get; set; }
        public object? Data { get; set; }
    }

    public class ResponseLoggingModel
    {
        public bool? Success { get; set; }
        public string? RequestUri { get; set; }
        public DateTime ResponseTime { get; set; }
        public object? ResponseData { get; set; }
    }
}