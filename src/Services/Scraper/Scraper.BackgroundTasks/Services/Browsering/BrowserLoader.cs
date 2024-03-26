using Essentials.Logging;
using Essentials.Logging.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using Scraper.BackgroundTasks.Abstractions.Services.Browsering;
using Scraper.BackgroundTasks.Options;

namespace Scraper.BackgroundTasks.Services.Browsering
{
    internal sealed class BrowserLoader : IBrowserLoader
    {
        private readonly IBrowser _browser;
        private readonly ILogger<BrowserLoader> _logger;
        private readonly PuppeteerSharpOptions _puppeteerSharpOptions;

        public BrowserLoader(IOptions<PuppeteerSharpOptions> puppeteerSharpSettings,
            ILogger<BrowserLoader> logger)
        {
            if (puppeteerSharpSettings?.Value == null)
                throw new ArgumentNullException(nameof(PuppeteerSharpOptions));

            _puppeteerSharpOptions = puppeteerSharpSettings.Value;
            _logger = logger;
            _browser = Task.Run(async () =>
            {
                return await LaunchAsync(new LaunchOptions()
                {
                    Headless = true,
                    ExecutablePath = puppeteerSharpSettings.Value.ExecutablePath,
                    Args = new string[] { "--no-sandbox" }
                });
            }).GetAwaiter()
              .GetResult();
        }

        public async Task<IPage> LoadPage(string url)
        {
            var page = await _browser.NewPageAsync();
            await page.GoToAsync(url, _puppeteerSharpOptions.PageLoadTimeout);
            return page;
        }

        private async Task<IBrowser> LaunchAsync(LaunchOptions options)
        {
            try
            {
                return await Puppeteer.LaunchAsync(options);
            }
            catch (Exception ex)
            {
                _logger.LogException(EventIds.Scraping, ex, "An error occurred while trying to load Puppeteer with options: {Options}.", options);
                throw;
            }
        }

        public void Dispose()
        {
            _browser?.Dispose();
        }
    }
}
