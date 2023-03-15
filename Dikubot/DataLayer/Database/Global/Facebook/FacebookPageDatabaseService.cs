using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Dikubot.DataLayer.Database.Global.Facebook;

public class FacebookPageService : GlobalMongoService<FacebookPage>
{
    public FacebookPageService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "FacebookPages";
    }

    private Dictionary<string, List<FBEvent>> FBEventDic = new Dictionary<string, List<FBEvent>>();

    public List<FBEvent> GetFBEvents(string id, HttpClient httpClient)
    {
        
        if (FBEventDic.ContainsKey(id))
        {
            return FBEventDic[id];
        }
        var accessToken = this.Get(m => m.Item.Id == id).Item.AccessToken;
        
        var requestUri = $"https://graph.facebook.com/v16.0/{id}/events?access_token={accessToken}";
        var result = httpClient.GetFromJsonAsync<FBEventRespons>(requestUri).Result.Data;
        FBEventDic.Add(id, result);
        return result;
    }
    
    public string GetFBPicture(string id, HttpClient httpClient)
    {
        
        var accessToken = this.Get(m => m.Item.Id == id)?.Item?.AccessToken;
        
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var link = $"https://graph.facebook.com/v16.0/{id}/picture?redirect=0&access_token={accessToken}";
            return httpClient.GetFromJsonAsync<FBPictureRespons>(link).Result.Data.Url;
            
        }
        return "";
    }
}