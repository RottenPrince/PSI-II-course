using Newtonsoft.Json;

namespace MVC
{
    public class APIHelper
    {
        private static readonly string _apiUrl = "http://localhost:5096/";

        private static HttpClient _client = new HttpClient();

        private static T? MakeRequest<T>(string endpoint, Func<HttpClient, string, Task<HttpResponseMessage>> f)
        {
            var apiResult = f(_client, _apiUrl + endpoint).Result;
            apiResult.EnsureSuccessStatusCode();
            var content = apiResult.Content.ReadAsStringAsync().Result;
            if(typeof(T) == typeof(string))
            {
                content = $"\"{content}\"";
            }
            return JsonConvert.DeserializeObject<T>(content);
        }

        public static T? Get<T>(string endpoint)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.GetAsync(url));
        }


        public static U? Post<T, U>(string endpoint, T data)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PostAsJsonAsync(url, data));
        }

        public static T? Delete<T>(string endpoint)
        {
            return MakeRequest<T>(endpoint, (client, url) => client.DeleteAsync(url));
        }

        public static U? Put<T, U>(string endpoint, T data)
        {
            return MakeRequest<U>(endpoint, (client, url) => client.PutAsJsonAsync(url, data));
        }
    }
}
