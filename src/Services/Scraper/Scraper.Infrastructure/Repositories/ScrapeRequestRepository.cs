using Microsoft.EntityFrameworkCore;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Infrastructure.Repositories
{
    public class ScrapeRequestRepository : IScrapeRequestRepository
    {
        private readonly ScraperContext _context;

        public ScrapeRequestRepository(ScraperContext context)
        {
            _context = context;
        }

        public async Task DeleteByIdAsync(long id, string userId)
        {
            var entity = await _context.ScrapeRequest
                .Where(x => x.Id == id && x.UserId == userId)
                .Include(x => x.UrlFound)
                .FirstOrDefaultAsync();

            if (entity != null)
            {
                entity.UrlFound.Clear();
                _context.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsCascadeAsync()
        {
            return await _context
                .ScrapeRequest
                .Include(x => x.UrlFound)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsAsync()
        {
            return await _context
                .ScrapeRequest
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task CreateRequestAsync(ScrapeRequest scrape)
        {
            _context.ChangeTracker.Clear();
            _context.ScrapeRequest.Add(scrape);
            await _context.SaveChangesAsync();
        }

        public async Task<ScrapeRequest> GetRequestWithTrackingAsync(long id, string userId, string url)
        {
            return await _context
                .ScrapeRequest
                .Include(x => x.UrlFound)
                .Where(x => x.Id == id && x.UserId == userId && x.Url == url)
                .FirstAsync();
        }

        public async Task UpdateRequestAsync(long id, string userId, string searchText)
        {
            _context.ChangeTracker.Clear();
            var scrape = _context
                .ScrapeRequest
                .Where(x => x.Id == id && x.UserId == userId)
                .AsNoTracking()
                .First();
            scrape.SearchText = searchText;
            _context.Update(scrape);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRequestAsync(ScrapeRequest scrape)
        {
            _context.ChangeTracker.Clear();
            _context.Update(scrape);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetByIdAsync(long id)
        {
            return await _context
                .ScrapeRequest
                .Where(x => x.Id == id)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
