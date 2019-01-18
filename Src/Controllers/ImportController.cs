using System;
using System.Net;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TvShowsAPI.WebApi.Services;

namespace TvShowsAPI.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IScrapperService _scrapperService;
        public ImportController(ILogger<ImportController> logger, IScrapperService scrapperService)
        {
            _logger = logger;
            _scrapperService = scrapperService;
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {

                BackgroundJob.Enqueue(() => _scrapperService.DoImportAsync());
                return Accepted();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        
    }
}
