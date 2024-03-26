using Microsoft.Extensions.Caching.Distributed;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using System.Text;
using System.Text.Json;

namespace Scraper.Infrastructure.Repositories
{
    public class ScrapeRequestCacheRepository : IScrapeRequestCacheRepository
    {
        private const string SCRAPE_REQUEST_CACHE_KEY = "ScrapeRequestItem";
        private readonly IDistributedCache _distributedCache;
        public ScrapeRequestCacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<ScrapeRequest> GetItem(long id)
        {
            var redisCache = await _distributedCache.GetAsync($"{SCRAPE_REQUEST_CACHE_KEY}{id}");
            if (redisCache != null)
            {
                var cachedString = Encoding.UTF8.GetString(redisCache);
                return JsonSerializer.Deserialize<ScrapeRequest>(cachedString) ??
                    throw new InvalidCastException("Cannot convert cache value to ScrapeRequest!");
            }
            return new ScrapeRequest(string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public async Task SetItem(ScrapeRequest item)
        {
            var serializedScrapeItem = JsonSerializer.Serialize(item);
            var redisScrapeList = Encoding.UTF8.GetBytes(serializedScrapeItem);
            var options = new DistributedCacheEntryOptions();

            var itemCacheKey = $"{SCRAPE_REQUEST_CACHE_KEY}{item.Id}";
            await _distributedCache.SetAsync(itemCacheKey, redisScrapeList, options);
        }

        public async Task RemoveItem(long id)
        {
            var itemCacheKey = $"{SCRAPE_REQUEST_CACHE_KEY}{id}";
            await _distributedCache.RemoveAsync(itemCacheKey);
        }

        public async Task SetItemList(IReadOnlyList<ScrapeRequest> scrapeRequestList)
        {
            string serializedscrapeItem;
            byte[] rediScrapeList;
            foreach (var item in scrapeRequestList)
            {
                serializedscrapeItem = JsonSerializer.Serialize(item);
                rediScrapeList = Encoding.UTF8.GetBytes(serializedscrapeItem);
                var options = new DistributedCacheEntryOptions();

                var itemCacheKey = $"{SCRAPE_REQUEST_CACHE_KEY}{item.Id}";
                await _distributedCache.SetAsync(itemCacheKey, rediScrapeList, options);
            }
        }
    }
}
