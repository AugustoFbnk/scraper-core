using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Abstractions.Domain.DomainServices.Services
{
    public interface IScrapeCacheManagementService
    {
        Task<bool> AlreadRegisteredAsync(long id, string userId, string url, string urlFound);
        Task UpdateCacheAsync(ScrapeRequest scrape);
    }
}
