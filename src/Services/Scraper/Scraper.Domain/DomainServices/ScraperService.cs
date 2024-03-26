using Essentials.Logging;
using Essentials.Logging.Extensions;
using Microsoft.Extensions.Logging;
using Scraper.Abstractions.Domain.DomainServices.Services;
using Scraper.Domain.Abstractions.DomainServices;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Domain.Dto;
using Scraper.Domain.Extensions;
using System.Text;

namespace Scraper.Domain.DomainServices
{
    public class ScraperService : IScraperService
    {
        private readonly ILogger<ScraperService> _logger;
        private readonly IScrapeCacheManagementService _scrapeRecordService;
        private readonly IScrapeRequestRepository _scrapeRepository;
        private readonly IFcmService _fcmService;

        public ScraperService(ILogger<ScraperService> logger,
            IScrapeCacheManagementService scrapeRecordService,
            IScrapeRequestRepository scrapeRepository,
            IFcmService fcmService)
        {
            _scrapeRecordService = scrapeRecordService;
            _scrapeRepository = scrapeRepository;
            _fcmService = fcmService;
            _logger = logger;
        }

        public async Task ScrapeUrlsAsync(GroupedRequest groupedRequest, IReadOnlyList<ScrapeRequest> requests, IReadOnlyList<ScrapePair> linksPair)
        {
            try
            {
                var founds = GetUrlsWithKeysMatch(groupedRequest, linksPair);
                await ProcessRequests(requests, founds);
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.Scraping, ex, "An error occured while scraping the following url: {Url}", groupedRequest.Url);
                throw;
            }
        }

        /// <summary>
        /// Get urls with keys match
        /// </summary>
        /// <param name="groupedRequest">Request grouped by URL.</param>
        /// <param name="hyperLinks">All hrefs found in URL</param>
        /// <returns>A dictionary where key: url found and values: keys matchs with request keys</returns>
        private Dictionary<string, IEnumerable<string>> GetUrlsWithKeysMatch(GroupedRequest groupedRequest, IReadOnlyList<ScrapePair> hyperLinks)
        {
            var founds = new Dictionary<string, IEnumerable<string>>();

            foreach (var linkPair in hyperLinks
                .Where(x => !string.IsNullOrEmpty(x.Url) && !string.IsNullOrEmpty(x.Text))
                .ToList())
            {
                var keyWordMatches = groupedRequest.GetContainingMatches(linkPair).ToList();
                if (!keyWordMatches.Any()) continue;

                founds[linkPair.Url] = founds.TryGetValue(linkPair.Url, out var existingValue)
                    ? existingValue.Concat(keyWordMatches)
                    : keyWordMatches;
            }
            return founds;
        }

        /// <summary>
        /// Iterate over all users requests of same URL, storing matches and notifying users.
        /// </summary>
        /// <param name="requests">Users requests</param>
        /// <param name="founds">URLs wich contains matches</param>
        private async Task ProcessRequests(IReadOnlyList<ScrapeRequest> requests, Dictionary<string, IEnumerable<string>> founds)
        {
            foreach (var request in requests)
            {
                var foundsByRequest = request.GetMatchedPair(founds);
                await ProcessRequesFoundKeys(request, foundsByRequest);
            }
        }

        /// <summary>
        /// Iterate over matches founds by user request, storing matches and notifying users.
        /// </summary>
        /// <param name="request">User request</param>
        /// <param name="foundsByRequest">Matches founds</param>
        private async Task ProcessRequesFoundKeys(ScrapeRequest request, Dictionary<string, IEnumerable<string>> foundsByRequest)
        {
            var newFounds = new StringBuilder();
            foreach (var foundByRequest in foundsByRequest.Where(found => found.Value.Any()))
            {
                if (await _scrapeRecordService.AlreadRegisteredAsync(request.Id, request.UserId, request.Url, foundByRequest.Key ?? string.Empty))
                    continue;
                
                request.RegisterUrlsFounds(foundsByRequest);
                await _scrapeRepository.UpdateRequestAsync(request);
                await _scrapeRecordService.UpdateCacheAsync(request);
                newFounds.Append(string.Join(',', foundByRequest.Value));

            }

            var newFoundsStr = newFounds.ToString();
            if (MustSendNotification(request, newFoundsStr))
                await _fcmService.SendPushNotification(request.PushNotificationToken!,
                    $"New updates were found in {request.Url}",
                    $"Keys found: {newFoundsStr} ");
        }

        private static bool MustSendNotification(ScrapeRequest request, string newFoundsStr) =>
            !string.IsNullOrWhiteSpace(request.PushNotificationToken)
            && !string.IsNullOrWhiteSpace(newFoundsStr);
    }
}
