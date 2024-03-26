namespace Scraper.API.Application.Models
{
    public class ScrapeRequestModel
    {
        public ScrapeRequestModel(string id, string userId, string url, string[] searchText, List<ScrapeUrlFoundModel> urlFound)
        {
            Id = id;
            UserId = userId;
            Url = url;
            SearchText = searchText;
            UrlFound = urlFound;
        }

        public string Id { get; set; }

        /// <summary>
        /// URL to scrappy
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Tenant(user) that has requested the scrappy
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Texts to scrappy in URL
        /// </summary>
        public string[] SearchText { get; set; }

        /// <summary>
        /// URLs found in scraped URL
        /// </summary>
        public List<ScrapeUrlFoundModel> UrlFound { get; set; }
    }
}
