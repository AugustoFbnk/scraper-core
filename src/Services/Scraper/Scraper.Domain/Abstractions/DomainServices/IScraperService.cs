using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Domain.Dto;

namespace Scraper.Domain.Abstractions.DomainServices
{
    public interface IScraperService
    {
        Task ScrapeUrlsAsync(GroupedRequest groupedRequest, IReadOnlyList<ScrapeRequest> requests, IReadOnlyList<ScrapePair> linksPair);
    }
}
