using System.Collections.Generic;

namespace Dikubot.DataLayer.Database.Global.Facebook;


public class FacebookPage : MainModel
{
    
    public FBPages Item { get; set; }
}

public class FBrespons
{
    public List<FBPages> data { get; set; }
}
    
public class FBPages
{
    public string access_token { get; set; }
    public string category { get; set; }
    public string name { get; set; }
    public string id { get; set; }
    public List<FBCategory> category_list { get; set; }
}
    
public class FBCategory
{
    public string id { get; set; }
    public string name { get; set; }
}
    
public class FBAccessToken
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public int expires_in { get; set; }
}

public class FBEventRespons
{
    public List<FBEvent> data { get; set; }
}
    
public class FBEvent
{
    public string description { get; set; }
    public string end_time { get; set; }
    public string start_time { get; set; }
    public string id { get; set; }
    public string name { get; set; }
}
    
public class FBPictureRespons
{
    public FBPicture data { get; set; }
}
    
public class FBPicture
{
    public string url { get; set; }
    public int height { get; set; }
    public int width { get; set; }
    public bool is_silhouette { get; set; }
}
    
public class FBidRespons
{
    public string id { get; set; }
}