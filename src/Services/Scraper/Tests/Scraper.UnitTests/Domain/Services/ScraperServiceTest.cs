using Microsoft.Extensions.Logging;
using NSubstitute;
using Scraper.Abstractions.Domain.DomainServices.Services;
using Scraper.Domain.AggregatesModel.ScraperAggregate;
using Scraper.Domain.DomainServices;
using Scraper.Domain.Dto;

namespace Scraper.UnitTests.Domain.Services
{
    [TestClass]
    public class ScraperServiceTest
    {

        [DataRow(0, 0, 0, true, "")]//ScrapeUrlsAsync_must_not_duplicate_url_found_record
        [DataRow(1, 1, 0, false, "")]//ScrapeUrlsAsync_must_store_url_found_record
        [DataRow(1, 1, 1, false, "xpto")]//ScrapeUrlsAsync_must_send_push_notification
        [TestMethod]
        public async Task ScrapeUrlsAsync_macro_scenarios(int updateRequestAsyncCalls,
            int updateCacheAsyncCalls,
            int sendPushNotificationCalls,
            bool alreadRegistered,
            string pushNotificationToken)
        {
            //Arrange
            var logger = Substitute.For<ILogger<ScraperService>>();
            var scrapeRecordService = Substitute.For<IScrapeCacheManagementService>();
            scrapeRecordService.AlreadRegisteredAsync(Arg.Any<long>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(alreadRegistered);

            var scrapeRepository = Substitute.For<IScrapeRequestRepository>();
            var fcmService = Substitute.For<IFcmService>();

            var url = "https://www.test_url.com/";
            var groupedRequest = new GroupedRequest
            {
                Url = url,
                SearchTexts = new[] { "search" }
            };

            var requestList = new List<ScrapeRequest> { new ScrapeRequest(Guid.NewGuid().ToString(), url, "search", pushNotificationToken) };
            var listPairLink = new List<ScrapePair> { new ScrapePair { Url = url, Text = "search" } };
            var scraperService = new ScraperService(logger, scrapeRecordService, scrapeRepository, fcmService);

            //Act
            await scraperService.ScrapeUrlsAsync(groupedRequest, requestList, listPairLink);

            //Assert
            await scrapeRepository.Received(updateRequestAsyncCalls).UpdateRequestAsync(Arg.Any<ScrapeRequest>());
            await scrapeRecordService.Received(updateCacheAsyncCalls).UpdateCacheAsync(Arg.Any<ScrapeRequest>());
            await fcmService.Received(sendPushNotificationCalls).SendPushNotification(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
