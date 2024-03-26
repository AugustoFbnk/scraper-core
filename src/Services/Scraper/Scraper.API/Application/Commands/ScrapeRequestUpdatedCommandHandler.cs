using Essentials;
using MediatR;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestUpdatedCommandHandler : IRequestHandler<ScrapeRequestUpdatedCommand>
    {
        private readonly IScrapeRequestRepository _scrapeRequestRepository;
        private readonly ILogger<ScrapeRequestCreatedCommandHandler> _logger;

        public ScrapeRequestUpdatedCommandHandler(IScrapeRequestRepository scrapeRequestRepository, ILogger<ScrapeRequestCreatedCommandHandler> logger)
        {
            _scrapeRequestRepository = scrapeRequestRepository;
            _logger = logger;
        }

        public async Task Handle(ScrapeRequestUpdatedCommand request, CancellationToken cancellationToken)
        {
            var searchText = String.Join(SC.WORD_DELIMITER, request.SearchText);
            await _scrapeRequestRepository.UpdateRequestAsync(request.Id, request.UserId, searchText);
        }
    }
}
