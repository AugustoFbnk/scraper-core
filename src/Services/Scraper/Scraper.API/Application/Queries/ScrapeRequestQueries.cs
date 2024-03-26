using Dapper;
using Essentials;
using Microsoft.Data.SqlClient;
using Scraper.API.Application.Models;

namespace Scraper.API.Application.Queries
{
    public class ScrapeRequestQueries : IScrapeRequestQueries
    {
        private readonly string _connString;

        public ScrapeRequestQueries(string connString)
        {
            _connString = connString ?? throw new ArgumentNullException("Connection string has not found!");
        }

        public async Task<List<ScrapeRequestModel>> GetByUserAsync(string userId)
        {
            using var connection = new SqlConnection(_connString);
            connection.Open();

            var result = await connection.QueryAsync<dynamic>(
                @"select sr.Id, sr.SearchText, sr.Url, suf.KeyWorkds as KeyWords, suf.UrlFound
                    from dbo.scrape_request sr
                    left join dbo.scrape_url_found suf on suf.ScrapeRequestId  = sr.Id 
                   where upper(sr.UserId)=upper(@userId)"
                    , new { userId }
                );

            return result.AsList().Any()
                ? MapScrapeRequest(userId, result)
                : new List<ScrapeRequestModel>();
        }

        private List<ScrapeRequestModel> MapScrapeRequest(string userId, IEnumerable<dynamic> result)
        {
            return result
                .GroupBy(x => new { x.Id, x.SearchText, x.Url })
                .Select(item => new ScrapeRequestModel(item.Key.Id,
                                                       userId,
                                                       item.Key.Url,
                                                       GetKeysList(item.Key.SearchText),
                                                       GetUrlsFound(result.Where(x => x.Id == item.Key.Id).ToList())))
                .ToList();
        }

        private List<ScrapeUrlFoundModel> GetUrlsFound(List<dynamic> urlsFound)
        {
            return (from item in urlsFound
                    where item.UrlFound != null && item.KeyWords != null
                    select new ScrapeUrlFoundModel(item.UrlFound.ToString(), GetKeysList(item?.KeyWords)))?.ToList() ?? new List<ScrapeUrlFoundModel>();
        }

        private static string[] GetKeysList(dynamic searchText) => (searchText as string)?.Split(SC.WORD_DELIMITER).ToArray() ?? Array.Empty<string>();
    }
}
