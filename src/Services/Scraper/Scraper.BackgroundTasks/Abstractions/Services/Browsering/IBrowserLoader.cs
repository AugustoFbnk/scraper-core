using PuppeteerSharp;

namespace Scraper.BackgroundTasks.Abstractions.Services.Browsering
{
    public interface IBrowserLoader : IDisposable
    {
        Task<IPage> LoadPage(string url);
    }
}
