using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using TvShowsAPI.WebApi.Configuration;
using TvShowsAPI.WebApi.Models;

namespace TvShowsAPI.WebApi.Services
{
    /// <summary>
    /// Service to communicate with external provider API
    /// </summary>
    public interface IExternalApiService
    {
        /// <summary>
        /// Retrieves a paginated list of shows from external provider
        /// </summary>
        /// <param name="pageNumber">number of desired page</param>
        /// <returns>A list of tv shows</returns>
        Task<IEnumerable<Show>> GetShowsAsync(int pageNumber);

        /// <summary>
        /// Retrieves cast information for a specific show from external provider
        /// </summary>
        /// <param name="showId">The internal Id of the show</param>
        /// <returns>A list of cast members</returns>
        Task<IEnumerable<CastMember>> GetCastForShowAsync(int showId);
    }

    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _client;
        private readonly ILogger<ExternalApiService> _logger;
        private readonly ExternalApiConfiguration _config;

        public ExternalApiService(HttpClient client, ILogger<ExternalApiService> logger, IOptions<ExternalApiConfiguration> config)
        {
            _client = client;
            _logger = logger;
            _config = config.Value;
        }

        public async Task<IEnumerable<Show>> GetShowsAsync(int pageNumber)
        {
            var resource = string.Format(_config.ShowEndpoint, pageNumber);
            _logger.LogDebug($"Sending request to {_client.BaseAddress}/{resource}");
            var response = await _client.GetAsync(resource);
            response.EnsureSuccessStatusCode(); //throw ex if statuscode != 200

            return await response.Content.ReadAsAsync<IEnumerable<Show>>();
        }

        public async Task<IEnumerable<CastMember>> GetCastForShowAsync(int showId)
        {
            var resource = string.Format(_config.CastEndpoint, showId);
            _logger.LogDebug($"Sending request to {_client.BaseAddress}/{resource}");

            var response = await _client.GetAsync(resource);
            response.EnsureSuccessStatusCode(); //throw ex if statuscode != 200

            var json = await response.Content.ReadAsAsync<JArray>();
            //Consider only the person element of the response, ignoring character info
            return json.Values<JObject>("person").Select(p => p.ToObject<CastMember>()).ToList();
        }
    }
}
