using MediatR;

namespace Scraper.API.Application.Commands
{
    public class ScrapeRequestCreatedCommand : IRequest
    {
        public ScrapeRequestCreatedCommand(string url,
            string userId,
            string pushNotificationToken,
            string[] searchText)
        {
            Url = url;
            UserId = userId;
            PushNotificationToken = pushNotificationToken;
            SearchText = searchText;
        }

        /// <summary>
        /// Urls to scrappy
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// Uuser that has requested the scrappy
        /// </summary>
        public string UserId { get; }

        /// <summary>
        /// Push notification token recipient
        /// </summary>
        public string PushNotificationToken { get; set; }

        /// <summary>
        /// Texts to scrappy in url
        /// </summary>
        public string[] SearchText { get; }

        public override string ToString()
        {
            return $"Url {Url} User {UserId} SearchText {string.Join(',', SearchText)} ";
        }
    }
}
