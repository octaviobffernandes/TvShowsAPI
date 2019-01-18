using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TvShowsAPI.WebApi.Context;

namespace TvShowsAPI.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ShowsController : Controller
    {
        private readonly IShowRepository _showRepository;
        private readonly ILogger<ShowsController> _logger;

        public ShowsController(IShowRepository showRepository, ILogger<ShowsController> logger)
        {
            _showRepository = showRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            try
            {
                var shows = await _showRepository.GetPaginatedAsync(page);
                return Ok(shows);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        
    }
}
