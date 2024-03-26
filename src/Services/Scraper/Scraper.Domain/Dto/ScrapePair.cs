namespace Scraper.Domain.Dto
{
    /// <summary>
    /// Pair obtained from href html tags
    /// </summary>
    public class ScrapePair
    {
        /// <summary>
        /// Text to scrape
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        ///URl to scrape
        /// </summary>
        public string Url { get; set; }

        public override string ToString()
        {
            return $"Text: {Text}. Url: {Url}";
        }
    }
}
