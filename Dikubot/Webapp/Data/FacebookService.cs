using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Dikubot.DataLayer.Database.Global.Facebook;
using Dikubot.DataLayer.Database.Global.Request;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Dikubot.Webapp.Data;

public class FacebookService
{

    private string _appId = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_ID");
    private string _appSecret = Environment.GetEnvironmentVariable("FACEBOOK_APP_SECRET");
    private string _clientId = Environment.GetEnvironmentVariable("FACEBOOK_CLIENT_ID");
    private string _graphApiVersion = "v16.0";
    
    private FacebookPageService FacebookPageService { get; set; }
    private NavigationManager NavigationManager { get; set; }
    private FacebookRequestMongoService FacebookRequestMongoService { get; set; }
    private HttpClient HttpClient { get; set; }
    private JsonService JsonService { get; set; }

    public FacebookService( HttpClient httpClient,
                            FacebookPageService facebookPageService, 
                            FacebookRequestMongoService facebookRequestMongoService,
                            NavigationManager navigationManager,
                            JsonService jsonService)
    {
        HttpClient = httpClient;
        JsonService = jsonService;
        FacebookPageService = facebookPageService;
        NavigationManager = navigationManager;
        FacebookRequestMongoService = facebookRequestMongoService;
    }
    
    private Dictionary<string, List<FBEvent>> FBEventDic = new Dictionary<string, List<FBEvent>>();

    public List<FBEvent> GetFBEvents(string pageId)
    {
        
        if (FBEventDic.ContainsKey(pageId))
        {
            return FBEventDic[pageId];
        }
        var accessToken = FacebookPageService.Get(m => m.PageId == pageId).AccessToken;

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var requestUri = $"https://graph.facebook.com/{_graphApiVersion}/{pageId}/events?access_token={accessToken}";

            FBRespons<FBEvent> result = (FBRespons<FBEvent>)JsonService.GetJson<FBRespons<FBEvent>>(requestUri);

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
        
        var accessToken = FacebookPageService.Get(m => m.PageId == pageId)?.AccessToken;
        
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            var link = $"https://graph.facebook.com/{_graphApiVersion}/{pageId}/picture?redirect=0&access_token={accessToken}";
            var result = (FBPictureRespons)JsonService.GetJson<FBPictureRespons>(link);

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
            $"https://www.facebook.com/{_graphApiVersion}/dialog/oauth?" +
            "client_id=" + _clientId +
            "&redirect_uri=" + redirectUri +
            "&auth_type=rerequest" +
            "&config_id=1372246313628981";

        NavigationManager.NavigateTo(link);
    }


    public List<FacebookPageModel> AddLongLivedAccessTokenToRequestDatabase(string redirectUri, string codeParameter, ContactModel contact = null)
    {
        return AddLongLivedAccessTokenToRequestDatabase(GetLongLivedPageAccessToken(redirectUri, codeParameter), contact);
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
                var shortlivedAccessTokenLink = $"https://graph.facebook.com/{_graphApiVersion}/oauth/access_token?client_id={_appId}&redirect_uri={redirectUri}&client_secret={_appSecret}&code={codeParameter}";

                FBAccessToken shortLivedAccessToken = HttpClient.GetFromJsonAsync<FBAccessToken>(shortlivedAccessTokenLink).Result;
                
                // Link to get the short lived User AccessToken
                var getFacebookUsersPagesLink = $"https://graph.facebook.com/{_graphApiVersion}/me/accounts?access_token={shortLivedAccessToken.AccessToken}";

                var _fbPagesList = HttpClient.GetFromJsonAsync<FBRespons<FBPage>>(getFacebookUsersPagesLink).Result.Data;

                // Link to get the Long lived User AccessToken
                var longLivedAccessTokenLink = $"https://graph.facebook.com/{_graphApiVersion}/oauth/access_token?grant_type=fb_exchange_token&" +
                            $"client_id={_appId}&" +
                            $"client_secret={_appSecret}&" +
                            $"fb_exchange_token={shortLivedAccessToken.AccessToken}";
                var longLivedUserAccessToken = HttpClient.GetFromJsonAsync<FBAccessToken>(longLivedAccessTokenLink).Result;
                
                    
                // Link to get the unique app User id
                var userIdLink = $"https://graph.facebook.com/me?fields=id&access_token={longLivedUserAccessToken.AccessToken}";
                var appScopedUserId = HttpClient.GetFromJsonAsync<FBidRespons>(userIdLink).Result.Id;

                //Link to get Long Lived Page Access Token and page info
                var longLivedPageAccessTokenLink = $"https://graph.facebook.com/{_graphApiVersion}/{appScopedUserId}/" +
                            $"accounts?access_token={longLivedUserAccessToken.AccessToken}";

                //List of FBPag

                return HttpClient.GetFromJsonAsync<FBRespons<FBPage>>(longLivedPageAccessTokenLink).Result.Data;
                
                
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
            if (FacebookPageService.Exists(m => m.PageId == facebookPageItem.Id))
            {
                facebookPageModel = FacebookPageService.Get(m => m.PageId == facebookPageItem.Id);
            }
            facebookPageModel.FbPageConverter(facebookPageItem);
            FacebookPageService.Upsert(facebookPageModel);
            list.Add(facebookPageModel);
        }
        return list;
    }
    
    public List<FacebookPageModel> AddLongLivedAccessTokenToRequestDatabase(List<FBPage> facebookPagesList, ContactModel contact = null)
    {
        List<FacebookPageModel> list = new List<FacebookPageModel>();
        contact ??= new ContactModel(){Name = "NA", Mail = "NA", Number = "NA"};
        
        //Add Facebook page to the Databases with Long Lived Access Token
        foreach (var facebookPageItem in facebookPagesList)
        {
            FacebookPageModel facebookPageModel = new FacebookPageModel();
            if (FacebookPageService.Exists(m => m.PageId == facebookPageItem.Id))
            {
                facebookPageModel = FacebookPageService.Get(m => m.PageId == facebookPageItem.Id);
            }
            facebookPageModel.FbPageConverter(facebookPageItem);
            var requestModel = new RequestModel<FacebookPageModel>();
            requestModel.RequestItem = facebookPageModel;
            requestModel.Contact = contact;
            FacebookRequestMongoService.Upsert(requestModel);
            list.Add(facebookPageModel);
        }
        return list;
    }
    
    
}