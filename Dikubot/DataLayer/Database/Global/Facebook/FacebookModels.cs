﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dikubot.DataLayer.Database.Global.Facebook;


public class FacebookPageModel : MainModel
{
   
    public string AccessToken { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string PageId { get; set; }
    
    public List<FBCategory> CategoryList { get; set; }

    public void FbPageConverter(FBPage item)
    {
        AccessToken = item.AccessToken;
        Category = item.Category;
        Name = item.Name;
        PageId = item.Id;
        CategoryList = item.CategoryList;
    }
}


public class FBPage
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("category")]
    public string Category { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("category_list")]
    public List<FBCategory> CategoryList { get; set; }
}




public class FBRespons<Tmodel>
{
    [JsonPropertyName("data")]
    public List<Tmodel> Data { get; set; }
}
    
public class FBCategory
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
    
public class FBAccessToken
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

    
public class FBEvent
{[JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("end_time")]
    public string EndTime { get; set; }
    [JsonPropertyName("start_time")]
    public string StartTime { get; set; }
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
    
public class FBPictureRespons
{[JsonPropertyName("data")]
    public FBPicture Data { get; set; }
}
    
public class FBPicture
{[JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("height")]
    public int Height { get; set; }
    [JsonPropertyName("width")]
    public int Width { get; set; }
    [JsonPropertyName("is_silhouette")]
    public bool IsSilhouette { get; set; }
}
    
public class FBidRespons
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}