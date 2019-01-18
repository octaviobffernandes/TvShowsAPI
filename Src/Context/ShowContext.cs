using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TvShowsAPI.WebApi.Configuration;
using TvShowsAPI.WebApi.Models;

namespace TvShowsAPI.WebApi.Context
{
    public interface IShowContext
    {
        IMongoCollection<Show> Shows { get; }
    }

    public class ShowContext : IShowContext
    {
        private readonly IMongoDatabase _db;
        public ShowContext(IOptions<MongoDbConfiguration> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.ShowsCollectionName);
        }
        public IMongoCollection<Show> Shows => _db.GetCollection<Show>("Show");

    }
}
