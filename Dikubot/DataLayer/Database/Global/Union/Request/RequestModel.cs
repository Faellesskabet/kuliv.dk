﻿namespace Dikubot.DataLayer.Database.Global.Union.Request;

public class RequestModel<TModel> : MainModel where TModel : MainModel, new()
{
    public enum StatusEmun
    {
        ongoing,
        change,
        denied,
        approved
    }


    /// <summary>
    ///     Contact info on the one who made the request
    /// </summary>
    public ContactModel Contact { get; set; } = new();

    /// <summary>
    ///     The Requset Item,
    /// </summary>
    public TModel RequestItem { get; set; } = new();

    /// <summary>
    ///     The Requestet item status
    /// </summary>
    public StatusEmun Status { get; set; } = StatusEmun.ongoing;
}