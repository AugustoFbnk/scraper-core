using Microsoft.Extensions.Logging;
using Scraper.Abstractions.Domain.DomainServices.Services;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Domain.DomainServices
{
    public class ScrapeCacheManagementService : IScrapeCacheManagementService
    {
        private readonly ILogger<ScrapeCacheManagementService> _logger;
        private readonly IScrapeRequestRepository _scrapeRepository;
        private readonly IScrapeRequestCacheRepository _scrapeRequestCacheRepository;

        public ScrapeCacheManagementService(ILogger<ScrapeCacheManagementService> logger,
            IScrapeRequestRepository scrapeRepository,
            IScrapeRequestCacheRepository scrapeRequestCacheRepository
            )
        {
            _scrapeRepository = scrapeRepository;
            _scrapeRequestCacheRepository = scrapeRequestCacheRepository;

            _scrapeRequestCacheRepository.SetItemList(_scrapeRepository.GetAllRequestsCascadeAsync().GetAwaiter().GetResult()).Wait();
            _logger = logger;
        }

        public async Task<bool> AlreadRegisteredAsync(long id, string userId, string url, string urlFound)
        {
            var item = await _scrapeRequestCacheRepository.GetItem(id);
            return item?.UserId == userId
                && item?.Url == url
                && (item?.UrlFound?.Exists(x => x?.UrlFound == urlFound.TrimEnd().TrimStart()) ?? false);
        }

        public async Task UpdateCacheAsync(ScrapeRequest scrape)
        {
            await _scrapeRequestCacheRepository.SetItem(scrape);
        }
    }
}
