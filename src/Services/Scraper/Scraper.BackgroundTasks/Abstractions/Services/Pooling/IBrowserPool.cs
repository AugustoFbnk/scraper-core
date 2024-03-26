namespace Scraper.BackgroundTasks.Abstractions.Services.Pooling
{
    public interface IBrowserPool : IDisposable
    {
        Task<IPoolObject> GetFreeObject(string url);
    }
}
