using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using TvShowsAPI.WebApi.Models;

namespace TvShowsAPI.WebApi.Context
{
    /// <summary>
    /// Repository class for show collection on internal database
    /// </summary>
    public interface IShowRepository
    {
        /// <summary>
        /// Get paginated list of Shows from database
        /// </summary>
        /// <param name="page">Page number</param>
        /// <returns>A list of show entities</returns>
        Task<IEnumerable<Show>> GetPaginatedAsync(int page);

        /// <summary>
        /// Save a batch of show entities to the database
        /// </summary>
        /// <param name="shows">The list of show entities</param>
        Task SaveBatchAsync(IEnumerable<Show> shows);
    }

    public class ShowRepository : IShowRepository
    {
        private IShowContext _showContext;
        private readonly int pageSize = 250;

        public ShowRepository(IShowContext showContext)
        {
            _showContext = showContext;
        }

        public async Task<IEnumerable<Show>> GetPaginatedAsync(int page)
        {
            var skip = pageSize * page;
            var results = await _showContext.Shows.WithReadPreference(ReadPreference.Secondary)
                .Find(_ => true)
                .Skip(skip)
                .Limit(pageSize)
                .ToListAsync();
            return results.OrderBy(r => r.Id);
        }

        public async Task SaveBatchAsync(IEnumerable<Show> shows)
        {
            var bulk = new List<WriteModel<Show>>();
            foreach (var show in shows)
            {
                var upsertOne = new ReplaceOneModel<Show>(Builders<Show>.Filter.Where(x => x.Id == show.Id), show)
                {
                    IsUpsert = true
                };

                bulk.Add(upsertOne);
            }
            await _showContext.Shows.BulkWriteAsync(bulk);
        }
    }
}
