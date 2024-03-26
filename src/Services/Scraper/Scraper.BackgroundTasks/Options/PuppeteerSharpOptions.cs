namespace Scraper.BackgroundTasks.Options
{
    public class PuppeteerSharpOptions
    {
        public const string OPTION_NAME = "PuppeteerSharp";
        public string ExecutablePath { get; set; } = string.Empty;
        public int PageLoadTimeout { get; set; } = default;
    }
}
