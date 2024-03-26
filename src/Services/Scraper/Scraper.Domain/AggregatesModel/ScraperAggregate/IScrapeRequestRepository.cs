using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ScraperAggregate
{
    public interface IScrapeRequestRepository : IRepository<ScrapeRequest>
    {
        Task DeleteByIdAsync(long id, string userId);
        Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsCascadeAsync();
        Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsAsync();
        Task CreateRequestAsync(ScrapeRequest scrape);
        Task UpdateRequestAsync(ScrapeRequest scrape);
        Task UpdateRequestAsync(long id, string userId, string searchText);
        Task<IReadOnlyList<ScrapeRequest>> GetByIdAsync(long id);
    }
}
