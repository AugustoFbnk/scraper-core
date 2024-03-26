namespace Scraper.Domain.Dto
{
    /// <summary>
    /// Contains an URL to scrape and a grouped list of text to scrape retrieved from multiple requests for the same url
    /// </summary>
    public class GroupedRequest
    {
        /// <summary>
        /// URl to scrape
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Text to scrape
        /// </summary>
        public IReadOnlyList<string> SearchTexts { get; set; }

        public override string ToString()
        {
            return $"Url: {Url}. SearchTexts: {string.Join(',', SearchTexts)}";
        }
    }
}
