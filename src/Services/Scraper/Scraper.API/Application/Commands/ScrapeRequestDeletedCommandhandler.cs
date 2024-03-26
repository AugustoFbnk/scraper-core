using MediatR;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestDeletedCommandhandler : IRequestHandler<ScrapeRequestDeletedCommand>
    {
        private readonly IScrapeRequestRepository _scrapeRequestRepository;
        private readonly IScrapeRequestCacheRepository _scrapeRequestCacheRepository;

        public ScrapeRequestDeletedCommandhandler(IScrapeRequestRepository scrapeRequestRepository,
            IScrapeRequestCacheRepository scrapeRequestCacheRepository
            )
        {
            _scrapeRequestRepository = scrapeRequestRepository;
            _scrapeRequestCacheRepository = scrapeRequestCacheRepository;
        }

        public async Task Handle(ScrapeRequestDeletedCommand request, CancellationToken cancellationToken)
        {
            await _scrapeRequestRepository.DeleteByIdAsync(request.Id, request.UserId);
            await _scrapeRequestCacheRepository.RemoveItem(request.Id);
        }
    }
}
