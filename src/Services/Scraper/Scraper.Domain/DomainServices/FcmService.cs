using Essentials.Logging;
using Essentials.Logging.Extensions;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;
using Scraper.Abstractions.Domain.DomainServices.Services;

namespace Scraper.Domain.DomainServices
{
    /// <summary>
    /// Firebase cloud messaging service
    /// </summary>
    public class FcmService : IFcmService
    {
        private readonly ILogger<FcmService> _logger;

        public FcmService(ILogger<FcmService> logger)
        {
            _logger = logger;
        }

        public async Task SendPushNotification(string token, string title, string body)
        {

            var message = new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Title = title,
                    Body = body
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation(EventIds.ScraperBackgroundTaskLayer, "Push notification was sent: {response}", response);
            }
            catch (FirebaseMessagingException e)
            {
                _logger.LogException(EventIds.ScraperBackgroundTaskLayer, e, "Push notification error");
            }
        }
    }
}
