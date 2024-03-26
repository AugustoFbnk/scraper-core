using PuppeteerSharp;
using Scraper.BackgroundTasks.Abstractions.Services.Pooling;

namespace Scraper.BackgroundTasks.Services.Pooling
{
    internal sealed class PoolObject : IPoolObject
    {
        public PoolObject(int id, DateTime createdDate, IPage page)
        {
            Id = id;
            CreatedDate = createdDate;
            Page = page;
        }

        public int Id { get; }

        public DateTime CreatedDate { get; }

        //TODO: Abstract page to unclouple puppetersharp
        public IPage Page { get; }

        public void Dispose()
        {
            Page?.Dispose();
        }
    }
}
