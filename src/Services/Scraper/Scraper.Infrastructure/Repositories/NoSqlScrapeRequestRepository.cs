using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Infrastructure.Repositories
{
    public class NoSqlScrapeRequestRepository : IScrapeRequestRepository
    {
        public async Task DeleteByIdAsync(long id, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsCascadeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task CreateRequestAsync(ScrapeRequest scrape)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRequestAsync(long id, string userId, string searchText)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRequestAsync(ScrapeRequest scrape)
        {
            throw new NotImplementedException();
        }
    }
}
