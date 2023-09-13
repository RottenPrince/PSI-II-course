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
            var forecast = JsonConvert.DeserializeObject<T>(apiResult.Content.ReadAsStringAsync().Result);
            return forecast;
        }
    }
}
