using Newtonsoft.Json;

namespace MVC
{
    public class APIHelper
    {
        private static readonly string _apiUrl = "http://localhost:5096/";

        private static HttpClient _client = new HttpClient();
        public static T? Get<T>(string endpoint)
        {
            var apiResult = _client.GetAsync(_apiUrl + endpoint).Result;
            apiResult.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<T>(apiResult.Content.ReadAsStringAsync().Result);
            return result;
        }

        public static U? Post<T, U>(string endpoint, T data)
        {
            var apiResult = _client.PostAsJsonAsync(_apiUrl + endpoint, data).Result;
            apiResult.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<U>(apiResult.Content.ReadAsStringAsync().Result);
            return result;
        }

        public static T? Delete<T>(string endpoint)
        {
            var apiResult = _client.DeleteAsync(_apiUrl + endpoint).Result;
            apiResult.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<T>(apiResult.Content.ReadAsStringAsync().Result);
            return result;
        }

        public static U? Put<T, U>(string endpoint, T data)
        {
            var apiResult = _client.PutAsJsonAsync(_apiUrl + endpoint, data).Result;
            apiResult.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<U>(apiResult.Content.ReadAsStringAsync().Result);
            return result;
        }
    }
}
