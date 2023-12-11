using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BrainBoxUI.Helpers.API
{
    public interface IApiRepository
    {
        T? Get<T>(string endpoint, bool includeBearerToken, out APIError? error);
        U? Post<T, U>(string endpoint, T data, bool includeBearerToken, out APIError? error);
        T? Delete<T>(string endpoint, bool includeBearerToken, out APIError? error);
        U? Put<T, U>(string endpoint, T data, bool includeBearerToken, out APIError? error);
    }

    public class ApiRepository : IApiRepository
    {
        private readonly string _apiUrl = "http://localhost:5096/";
        private readonly HttpClient _client = new HttpClient();
        private readonly IHttpContextAccessor _httpContext;

        public ApiRepository(IHttpContextAccessor httpContext)
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpContext = httpContext;
        }

        private String GetBearerToken()
        {
            var pair = _httpContext.HttpContext.Request.Cookies.FirstOrDefault(
                    pr => pr.Key == "BearerToken"
                );
            if (pair.Equals(default(KeyValuePair<String, String>))) return null;
            return pair.Value;
        }

        private T? MakeRequest<T>(string endpoint, Func<HttpClient, string, HttpResponseMessage> requestFunc, bool includeBearerToken, out APIError? error)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + endpoint);

            var bearerToken = GetBearerToken();
            if (includeBearerToken && bearerToken != null)
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
            }

            var apiResult = requestFunc(_client, _apiUrl + endpoint);
            var content = apiResult.Content.ReadAsStringAsync().Result;

            if (!apiResult.IsSuccessStatusCode)
            {
                if(content.Length > 2 && content[0] == '"' && content[content.Length - 1] == '"')
                {
                    content = content.Substring(1, content.Length - 2);
                }
                error = new APIError(apiResult.StatusCode, content);
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                if (content.Length == 0 || content[0] != '"')
                {
                    content = $"\"{content}\"";
                }
            }

            error = null;
            return JsonConvert.DeserializeObject<T>(content);
        }

        public T? Get<T>(string endpoint, bool includeBearerToken, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.GetAsync(url).Result, includeBearerToken, out error);
        }

        public U? Post<T, U>(string endpoint, T data, bool includeBearerToken, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PostAsJsonAsync(url, data).Result, includeBearerToken, out error);
        }

        public T? Delete<T>(string endpoint, bool includeBearerToken, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.DeleteAsync(url).Result, includeBearerToken, out error);
        }

        public U? Put<T, U>(string endpoint, T data, bool includeBearerToken, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PutAsJsonAsync(url, data).Result, includeBearerToken, out error);
        }
    }
}