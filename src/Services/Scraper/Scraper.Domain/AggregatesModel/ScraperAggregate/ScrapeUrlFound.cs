using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ScraperAggregate
{
    public class ScrapeUrlFound : Entity
    {
        public string ScrapeRequestId { get; set; }
        public string UrlFound { get; set; }
        public string KeyWorkds { get; set; }
    }
}
