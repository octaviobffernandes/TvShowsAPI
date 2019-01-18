namespace TvShowsAPI.WebApi.Configuration
{
    public class MongoDbConfiguration
    {
        public string ConnectionString { get; set; }
        public string HangfireCollectionName { get; set; }
        public string ShowsCollectionName { get; set; }
    }
}
