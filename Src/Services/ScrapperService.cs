using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using TvShowsAPI.WebApi.Context;

namespace TvShowsAPI.WebApi.Services
{
    /// <summary>
    /// Internal scraper service which coordinates the scraping routine
    /// </summary>
    public interface IScrapperService
    {
        /// <summary>
        /// Long running process that scraps the whole database of shows/cast from external service
        /// Recommended to be called in a background job/task
        /// </summary>
        /// <returns></returns>
        Task DoImportAsync();
    }

    public class ScrapperService : IScrapperService
    {
        private readonly IShowRepository _showRepository;
        private readonly ILogger<ScrapperService> _logger;
        private readonly IExternalApiService _client;

        public ScrapperService(IShowRepository showRepository, ILogger<ScrapperService> logger, IExternalApiService client)
        {
            _showRepository = showRepository;
            _logger = logger;
            _client = client;
        }

        public async Task DoImportAsync()
        {
            var mustContinue = true;
            var pageNumber = 42;

            do
            {
                //get show and cast info from external service
                _logger.LogInformation($"Requesting shows for third party provider - page {pageNumber}");
                var shows = (await _client.GetShowsAsync(pageNumber)).ToList();
                if (!shows.Any())
                {
                    _logger.LogInformation($"No more shows returned - terminating communication with third party provider.");
                    mustContinue = false;
                }
                foreach(var show in shows)
                    show.Cast = (await _client.GetCastForShowAsync(show.Id)).ToList();

                //Save retrieved info to internal db
                _logger.LogInformation($"{shows.Count} shows returned - writing to local database.");
                await _showRepository.SaveBatchAsync(shows);
                pageNumber++;
            }
            while (mustContinue);

            _logger.LogInformation($"Finished import process.");            
        }
    }
}
