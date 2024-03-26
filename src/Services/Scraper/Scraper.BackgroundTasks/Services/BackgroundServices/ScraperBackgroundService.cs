using Essentials.Logging;
using Essentials.Logging.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using Scraper.BackgroundTasks.Abstractions.Services.Pooling;
using Scraper.BackgroundTasks.Services.Pooling;
using Scraper.Domain.Abstractions.DomainServices;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Domain.Dto;

namespace Scraper.BackgroundTasks.Services.BackgroundServices
{
    internal class ScraperBackgroundService : BackgroundService
    {
        private ILogger<ScraperBackgroundService> _logger;
        private readonly IScraperService _scraperService;
        private readonly IScrapeRequestRepository _scrapeRequestRepository;
        private readonly IBrowserPool _browserPool;
        private readonly List<string> _crashedUrls;

        public ScraperBackgroundService(ILogger<ScraperBackgroundService> scraperBackgroundServiceLogger,
            IScraperService scraperService,
            IScrapeRequestRepository scrapeRequestRepository,
            IBrowserPool browserPool)
        {
            _logger = scraperBackgroundServiceLogger;
            _scraperService = scraperService;
            _scrapeRequestRepository = scrapeRequestRepository;
            _browserPool = browserPool;
            _crashedUrls = new List<string>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation(EventIds.LifeCycle, "Starting service {ScraperBackgroundService}", nameof(ScraperBackgroundService));
                await DoScraperTask(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.ScraperBackgroundTaskLayer, ex, "An error occured while executing {ScraperBackgroundService}.", nameof(ScraperBackgroundService));
                throw new Exception("Oh God! No!", ex);
            }
            finally
            {
                _logger.LogWarning(EventIds.LifeCycle, "Service {ScraperBackgroundService} is turning down", nameof(ScraperBackgroundService));
                _browserPool?.Dispose();
            }
        }

        private async Task DoScraperTask(CancellationToken stoppingToken)
        {
            while (true && !stoppingToken.IsCancellationRequested)
            {
                var requests = (await _scrapeRequestRepository.GetAllRequestsAsync())
                    .Where(x => !_crashedUrls.Contains(x.Url))
                    .ToList();

                var groupedSearchTexts = GroupByUrl(requests);
                await ScrapeRequests(requests, groupedSearchTexts);

                await Task.Delay(20000, stoppingToken);
            }
        }

        private List<GroupedRequest> GroupByUrl(IEnumerable<ScrapeRequest> requests) =>
            requests
            .GroupBy(sr => sr.Url)
            .Select(grp => new GroupedRequest { Url = grp.Key, SearchTexts = grp.SelectMany(sr => sr.GetSearchTextArray()).ToList() })
            .ToList();


        private async Task ScrapeRequests(List<ScrapeRequest> requests, List<GroupedRequest> groupedSearchTexts)
        {
            string currentUrl = string.Empty;
            try
            {
                foreach (var groupedRequest in groupedSearchTexts)
                {
                    currentUrl = groupedRequest.Url;
                    var poolItem = await _browserPool.GetFreeObject(groupedRequest.Url);
                    var hyperLinks = await GetAllHyperLinks(poolItem.Page);

                    if (!hyperLinks.Any()) { continue; }

                    await _scraperService.ScrapeUrlsAsync(groupedRequest, requests.Where(x => x.Url == groupedRequest.Url).ToList(), hyperLinks);
                }
            }
            catch (NavigationException ne)
            {
                _crashedUrls.Add(currentUrl);
                _logger.LogException(EventIds.ScraperBackgroundTaskLayer,
                    ne,
                    "An error occured while executing {ScraperBackgroundService}.",
                    nameof(BrowserPool));
            }
        }

        private async Task<List<ScrapePair>> GetAllHyperLinks(IPage page)
            => (await page.EvaluateExpressionAsync<ScrapePair[]>(XPath.GET_TEXT_AND_HREF_EXPRESSION))?.ToList()
            ?? new List<ScrapePair>();

    }
}
