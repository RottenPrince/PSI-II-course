using Newtonsoft.Json;

namespace BrainBoxUI.Helpers.API
{
    public interface IApiRepository
    {
        T? Get<T>(string endpoint, out APIError? error);
        U? Post<T, U>(string endpoint, T data, out APIError? error);
        T? Delete<T>(string endpoint, out APIError? error);
        U? Put<T, U>(string endpoint, T data, out APIError? error);
    }

    public class ApiRepository : IApiRepository
    {
        private readonly string _apiUrl = "http://localhost:5096/";
        private readonly HttpClient _client = new HttpClient();

        private T? MakeRequest<T>(string endpoint, Func<HttpClient, string, HttpResponseMessage> f, out APIError? error)
        {
            var apiResult = f(_client, _apiUrl + endpoint);
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

        public T? Get<T>(string endpoint, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.GetAsync(url).Result, out error);
        }

        public U? Post<T, U>(string endpoint, T data, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PostAsJsonAsync(url, data).Result, out error);
        }

        public T? Delete<T>(string endpoint, out APIError? error)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.DeleteAsync(url).Result, out error);
        }

        public U? Put<T, U>(string endpoint, T data, out APIError? error)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PutAsJsonAsync(url, data).Result, out error);
        }
    }
}
