using Microsoft.Extensions.Logging;

namespace Essentials.Logging
{
    public class EventIds
    {
        public readonly static EventId LifeCycle = new(1000, "LifeCycle");

        public readonly static EventId ScraperBackgroundTaskLayer = new(2000, "ScraperBackgroundTask");

        public readonly static EventId ScraperAPILayer = new(2100, "ScraperAPI");

        public readonly static EventId Scraping = new(3000, "Scraping");
    }
}
