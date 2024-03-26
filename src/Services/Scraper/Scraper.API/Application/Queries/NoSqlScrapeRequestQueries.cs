using Google.Cloud.Firestore;
using Scraper.API.Application.Models;

namespace Scraper.API.Application.Queries
{
    public class NoSqlScrapeRequestQueries : IScrapeRequestQueries
    {
        private readonly FirestoreDb _context;

        public NoSqlScrapeRequestQueries(FirestoreDb context)
        {
            _context = context;
        }

        public async Task<List<ScrapeRequestModel>> GetByUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        private List<ScrapeRequestModel> MapScrapeRequest(string userId, IEnumerable<dynamic> result)
        {
            throw new NotImplementedException();
        }

        private List<ScrapeUrlFoundModel> GetUrlsFound(List<dynamic> urlsFound)
        {
            throw new NotImplementedException();
        }

        private static string[] GetKeysList(dynamic searchText) => throw new NotImplementedException();
    }
}
