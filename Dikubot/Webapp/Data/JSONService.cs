#nullable enable
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Dikubot.Webapp.Data;

public class JsonService
{
    private HttpClient _httpClient;
    
    public JsonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public object? GetJson<Tmodel>(string requestUri)
    {
        var res = _httpClient.GetAsync(requestUri);
        if (res.Result.IsSuccessStatusCode)
        {
                var stringData = res.Result.Content.ReadAsStringAsync();
                return (Tmodel) JsonConvert.DeserializeObject<Tmodel>(stringData.Result); 
        }
        else
        {
            return null;
        }
    }
}
    
