namespace Dikubot.DataLayer.Database.Global
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class GlobalServices<TModel> : Services<TModel> where TModel : MainModel
    {
        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected GlobalServices(string collectionName) : base($"KULIV_GLOBAL", collectionName)
        { }
    }
}