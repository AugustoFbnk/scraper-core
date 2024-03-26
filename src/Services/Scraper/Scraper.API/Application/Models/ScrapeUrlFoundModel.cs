namespace Scraper.API.Application.Models
{
    public class ScrapeUrlFoundModel
    {
        public ScrapeUrlFoundModel(string UrlFound, string[] KeyWords)
        {
            this.UrlFound = UrlFound;
            this.KeyWords = KeyWords;
        }

        /// <summary>
        ///Url found in some scrape request
        /// </summary>
        public string UrlFound { get; set; }

        /// <summary>
        /// Texts found in URL
        /// </summary>
        public string[] KeyWords { get; set; }
    }
}
