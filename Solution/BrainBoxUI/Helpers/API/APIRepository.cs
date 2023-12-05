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
        void SetBearerToken(string bearerToken);
        string GetBearerToken();
    }

    public class ApiRepository : IApiRepository
    {
        private readonly string _apiUrl = "http://localhost:5096/";
        private readonly HttpClient _client = new HttpClient();

        private string? _bearerToken;

        public ApiRepository()
        {
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetBearerToken(string bearerToken)
        {
            _bearerToken = bearerToken;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        public string GetBearerToken()
        {
            return _bearerToken;
        }

        private T? MakeRequest<T>(string endpoint, Func<HttpClient, string, HttpResponseMessage> requestFunc, bool includeBearerToken, out APIError? error)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _apiUrl + endpoint);

            if (includeBearerToken && _bearerToken != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            }




            var apiResult = requestFunc(_client, _apiUrl + endpoint);
            var content = apiResult.Content.ReadAsStringAsync().Result;

            if (!apiResult.IsSuccessStatusCode)
            {
                error = new APIError(apiResult.StatusCode, content);
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                content = $"\"{content}\"";
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