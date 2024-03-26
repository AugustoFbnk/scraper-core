using PuppeteerSharp;

namespace Scraper.BackgroundTasks.Abstractions.Services.Pooling
{
    public interface IPoolObject : IDisposable
    {
        int Id { get; }
        DateTime CreatedDate { get; }
        IPage Page { get; }
    }
}
