using Essentials;
using Scraper.Domain.SeedWork;

namespace Scraper.Domain.AggregatesModel.ScraperAggregate
{
    /// <summary>
    /// Represents an item to scrappy requested by any user
    /// </summary>
    public class ScrapeRequest : Entity, IAggregateRoot
    {
        public ScrapeRequest(string userId,
            string url,
            string searchText,
            string? pushNotificationToken)
        {
            UserId = userId;
            Url = url;
            SearchText = searchText;
            PushNotificationToken = pushNotificationToken;
            UrlFound = new List<ScrapeUrlFound>();
        }

        /// <summary>
        /// Urls to scrappy
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// User that has requested the scrappy
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Push notification token recipient
        /// </summary>
        public string? PushNotificationToken { get; set; }

        /// <summary>
        /// Texts to scrappy in url
        /// </summary>
        public string SearchText { get; set; }

        public List<ScrapeUrlFound> UrlFound { get; set; }

        public string[] GetSearchTextArray()
        {
            return SearchText
                .Split(SC.WORD_DELIMITER)
                .Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                .ToArray();
        }

        public Dictionary<string, IEnumerable<string>> GetMatchedPair(Dictionary<string, IEnumerable<string>> founds)
           => founds
               .Select(found => new { found.Key, Value = found.Value.Intersect(GetSearchTextArray()) })
               .Where(found => found.Value.Any())
               .ToDictionary(found => found.Key, found => found.Value);

        /// <summary>
        /// Iterate over matches founds by user request, storing matches and notifying users.
        /// </summary>
        /// <param name="request">User request</param>
        /// <param name="founds">Matches founds</param>
        /// <returns></returns>
        public void RegisterUrlsFounds(Dictionary<string, IEnumerable<string>> founds)
        {
            foreach (var found in founds)
            {
                var newUrlFound = new ScrapeUrlFound()
                {
                    UrlFound = found.Key?.TrimEnd().TrimStart() ?? string.Empty,
                    KeyWorkds = string.Join(SC.WORD_DELIMITER, found.Value.ToList())
                };
                UrlFound.Add(newUrlFound);
            }
        }
    }
}
