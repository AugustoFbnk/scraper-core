using Scraper.API.Application.Models;

namespace Scraper.API.Application.Queries
{
    public interface IScrapeRequestQueries
    {
        /// <summary>
        /// Return all requests by user
        /// </summary>
        /// <param name="userId">User's ID</param>
        /// <returns></returns>
        Task<List<ScrapeRequestModel>> GetByUserAsync(string userId);
    }
}
