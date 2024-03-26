using Essentials.Logging;
using Essentials.Logging.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Scraper.API.Application.Commands;
using Scraper.API.Application.Models;
using Scraper.API.Application.Queries;
using System.Net;

namespace Scraper.API.Controllers
{
    // [Authorize]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ScrapeRequestController : ControllerBase
    {
        private readonly ILogger<ScrapeRequestController> _logger;
        private readonly IMediator _mediator;
        private readonly IScrapeRequestQueries _scrapeRequestQueries;

        public ScrapeRequestController(ILogger<ScrapeRequestController> logger,
            IMediator mediator,
            IScrapeRequestQueries
            scrapeRequestQueries)
        {
            _logger = logger;
            _mediator = mediator;
            _scrapeRequestQueries = scrapeRequestQueries;
        }

        [HttpGet]
        [Route("Get")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<IEnumerable<ScrapeRequestModel>>> GetByUserAsync(string userId)
        {
            try
            {
                _logger.LogInformation(EventIds.LifeCycle, "Starting scrape requests query for user {UserId}", userId);

                var list = await _scrapeRequestQueries.GetByUserAsync(userId);

                _logger.LogInformation(EventIds.LifeCycle, "Finished scrape requests query for user {UserId}", userId);

                return list.Any() ? Ok(list) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.ScraperAPILayer,
                    ex,
                    "An error occured while querying scrape requests for user {UserId}. Error: {ExceptionMessge} {InnerExceptioneMessage} {StackTrace}",
                    userId);

                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Create")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateRequestAsync([FromBody] ScrapeRequestCreatedCommand request)
        {
            try
            {
                _logger.LogInformation(EventIds.LifeCycle, "Starting scrape request create with the following args: {Request}", request);

                await _mediator.Send(request);

                _logger.LogInformation(EventIds.LifeCycle, "Finished scrape request create with the following args: {Request}", request);

                return CreatedAtAction("CreateRequest", request);
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.ScraperAPILayer,
                    ex,
                    "An error occured while creating scrape requestwith the following args: {Request}.",
                    request.ToString());

                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("Delete")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> DeleteRequestAsync(long id, string userId)
        {
            try
            {
                _logger.LogInformation(EventIds.LifeCycle, "Starting scrape request delete with id: {Id} and user: {UserId}", id, userId);

                var request = new ScrapeRequestDeletedCommand(id, userId);
                await _mediator.Send(request);

                _logger.LogInformation(EventIds.LifeCycle, "Finished scrape request delete with id: {Id} and user: {UserId}", id, userId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.ScraperAPILayer,
                    ex,
                    "An error occured while deleting a scrape request  with id: {Id} and use: {UserId}.",
                   id,
                   userId);

                return BadRequest();
            }
        }

        [HttpPatch]//Might be put in this case
        [Route("Update")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateRequestAsync([FromBody] ScrapeRequestUpdatedCommand request)
        {
            try
            {
                _logger.LogInformation(EventIds.LifeCycle, "Starting scrape request update with request: {Request}", request);

                await _mediator.Send(request);

                _logger.LogInformation(EventIds.LifeCycle, "Finished scrape request update with request: {Request}", request);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.ScraperAPILayer,
                    ex,
                    "An error occured while updating a scrape request with request: {Request}.",
                    request);
                return BadRequest();
            }
        }
    }
}
