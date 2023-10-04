using System.Text.Json;

namespace MVC
{
    public static class APIHelper
    {
        private static readonly string _apiUrl = "http://localhost:5096/";

        private static HttpClient _client = new HttpClient();

        private static T? MakeRequest<T>(string endpoint, Func<HttpClient, string, Task<HttpResponseMessage>> f, out APIError? error)
        {
            var apiResult = f(_client, _apiUrl + endpoint).Result;
            var content = apiResult.Content.ReadAsStringAsync().Result;
            if(!apiResult.IsSuccessStatusCode)
            {
                error = new APIError(apiResult.StatusCode, content);
                return default(T);
            }
            if(typeof(T) == typeof(string))
            {
                content = $"\"{content}\"";
            }
            error = null;
            return JsonSerializer.Deserialize<T>(content);
        }

        public static T? Get<T>(string endpoint, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.GetAsync(url), out error);
        }

        public static U? Post<T, U>(string endpoint, T data, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PostAsJsonAsync(url, data), out error);
        }

        public static T? Delete<T>(string endpoint, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.DeleteAsync(url), out error);
        }

        public static U? Put<T, U>(string endpoint, T data, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PutAsJsonAsync(url, data), out error);
        }
    }
}
