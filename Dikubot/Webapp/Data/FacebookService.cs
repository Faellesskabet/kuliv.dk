using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Dikubot.DataLayer.Database.Global.Facebook;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Dikubot.Webapp.Data;

public class FacebookService
{

    private string appId = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_ID");
    private string appSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET");
    private string clientId = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_ID");
    private string graphApiVersion = "v16.0";
    
    private FacebookPageService _facebookPageService { get; set; }
    private NavigationManager _navigationManager { get; set; }
    private HttpClient _httpClient { get; set; }
    private JsonService _jsonService { get; set; }

    public FacebookService( HttpClient httpClient,
                            FacebookPageService facebookPageService, 
                            NavigationManager navigationManager,
                            JsonService jsonService)
    {
        _httpClient = httpClient;
        _jsonService = jsonService;
        _facebookPageService = facebookPageService;
        _navigationManager = navigationManager;
    }
    
    private Dictionary<string, List<FBEvent>> FBEventDic = new Dictionary<string, List<FBEvent>>();

    public List<FBEvent> GetFBEvents(string pageId)
    {
        
        if (FBEventDic.ContainsKey(pageId))
        {
            return FBEventDic[pageId];
        }
        var accessToken = _facebookPageService.Get(m => m.PageId == pageId).AccessToken;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var requestUri = $"https://graph.facebook.com/{graphApiVersion}/{pageId}/events?access_token={accessToken}";

            FBRespons<FBEvent> result = (FBRespons<FBEvent>)_jsonService.GetJson<FBRespons<FBEvent>>(requestUri);

            if (result != null)
            {
                FBEventDic.Add(pageId, result.Data);
                return result.Data;
            }
        }
        
        return new List<FBEvent>();
    }
    
    public string GetFBPicture(string pageId)
    {
        
        var accessToken = _facebookPageService.Get(m => m.PageId == pageId)?.AccessToken;
        
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var link = $"https://graph.facebook.com/{graphApiVersion}/{pageId}/picture?redirect=0&access_token={accessToken}";
            var result = (FBPictureRespons)_jsonService.GetJson<FBPictureRespons>(link);

            if (result != null)
            {
                return result.Data.Url;
            }

        }
        return "";
    }

    public void ConnectFacebookAction(string redirectUri)
    {
        string link =
            $"https://www.facebook.com/{graphApiVersion}/dialog/oauth?" +
            "client_id=" + clientId +
            "&redirect_uri=" + redirectUri +
            "&auth_type=rerequest" +
            "&config_id=1372246313628981";

        _navigationManager.NavigateTo(link);
    }


    public List<FacebookPageModel> AddLongLivedAccessTokenToDatabase(string redirectUri, string codeParameter)
    {
        return AddLongLivedAccessTokenToDatabase(GetLongLivedPageAccessToken(redirectUri, codeParameter));
    }
    
     public List<FBPage> GetLongLivedPageAccessToken(string redirectUri, string codeParameter)
    {
        try
            {
                
                // Link to get the short lived User AccessToken
                var shortlivedAccessTokenLink = $"https://graph.facebook.com/{graphApiVersion}/oauth/access_token?client_id={appId}&redirect_uri={redirectUri}&client_secret={appSecret}&code={codeParameter}";

                FBAccessToken shortLivedAccessToken = _httpClient.GetFromJsonAsync<FBAccessToken>(shortlivedAccessTokenLink).Result;
                
                // Link to get the short lived User AccessToken
                var getFacebookUsersPagesLink = $"https://graph.facebook.com/{graphApiVersion}/me/accounts?access_token={shortLivedAccessToken.AccessToken}";

                var _fbPagesList = _httpClient.GetFromJsonAsync<FBRespons<FBPage>>(getFacebookUsersPagesLink).Result.Data;

                // Link to get the Long lived User AccessToken
                var longLivedAccessTokenLink = $"https://graph.facebook.com/{graphApiVersion}/oauth/access_token?grant_type=fb_exchange_token&" +
                            $"client_id={appId}&" +
                            $"client_secret={appSecret}&" +
                            $"fb_exchange_token={shortLivedAccessToken.AccessToken}";
                var longLivedUserAccessToken = _httpClient.GetFromJsonAsync<FBAccessToken>(longLivedAccessTokenLink).Result;
                
                    
                // Link to get the unique app User id
                var userIdLink = $"https://graph.facebook.com/me?fields=id&access_token={longLivedUserAccessToken.AccessToken}";
                var appScopedUserId = _httpClient.GetFromJsonAsync<FBidRespons>(userIdLink).Result.Id;

                //Link to get Long Lived Page Access Token and page info
                var longLivedPageAccessTokenLink = $"https://graph.facebook.com/{graphApiVersion}/{appScopedUserId}/" +
                            $"accounts?access_token={longLivedUserAccessToken.AccessToken}";

                //List of FBPag

                return _httpClient.GetFromJsonAsync<FBRespons<FBPage>>(longLivedPageAccessTokenLink).Result.Data;
                
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
    }
    
    

    public List<FacebookPageModel> AddLongLivedAccessTokenToDatabase(List<FBPage> facebookPagesList)
    {
        List<FacebookPageModel> list = new List<FacebookPageModel>();
        //Add Facebook page to the Databases with Long Lived Access Token
        foreach (var facebookPageItem in facebookPagesList)
        {
            FacebookPageModel facebookPageModel = new FacebookPageModel();
            if (_facebookPageService.Exists(m => m.PageId == facebookPageItem.Id))
            {
                facebookPageModel = _facebookPageService.Get(m => m.PageId == facebookPageItem.Id);
            }
            facebookPageModel.FbPageConverter(facebookPageItem);
            _facebookPageService.Upsert(facebookPageModel);
            list.Add(facebookPageModel);
        }
        return list;
    }
}