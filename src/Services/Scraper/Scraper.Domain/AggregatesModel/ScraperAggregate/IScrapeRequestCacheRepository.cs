namespace Scraper.Domain.AggregatesModel.ScraperAggregate
{
    public interface IScrapeRequestCacheRepository
    {
        Task<ScrapeRequest> GetItem(long id);
        Task SetItem(ScrapeRequest item);
        Task RemoveItem(long id);
        Task SetItemList(IReadOnlyList<ScrapeRequest> scrapeRequestList);
    }
}
