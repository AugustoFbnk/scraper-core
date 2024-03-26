using Essentials.Web.Extensions;
using Scraper.BackgroundTasks.Abstractions.Services.Browsering;
using Scraper.BackgroundTasks.Abstractions.Services.Pooling;

namespace Scraper.BackgroundTasks.Services.Pooling
{
    internal sealed class BrowserPool : IBrowserPool
    {
        private const int ITEM_LIFE_TIME = 3;
        private const int MAX_POOL_SIZE = 5;
        private readonly IBrowserLoader _loader;
        private List<IPoolObject> _poolItens;

        static readonly SemaphoreSlim semaphore = new(1);

        internal BrowserPool(IBrowserLoader loader, List<IPoolObject> poolItens)
        {
            _loader = loader;
            _poolItens = poolItens;
        }

        public BrowserPool(IBrowserLoader loader)
        {
            _poolItens = new List<IPoolObject>();
            _loader = loader;
        }

        public async Task<IPoolObject> GetFreeObject(string url)
        {
            await semaphore.WaitAsync();
            try
            {
                RecyclePool();

                var existingPoolItem = GetExistingItem(url);

                if (IsOldItem(existingPoolItem))
                {
                    DisposeItem(existingPoolItem!);
                    return await Create(url);
                }

                if (IsRecentlyCreatedItem(existingPoolItem))
                {
                    return existingPoolItem!;
                }

                return await Create(url);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private void RecyclePool()
        {
            while (_poolItens.Count > MAX_POOL_SIZE)
            {
                var oldPoolItem = _poolItens.OrderBy(x => x.CreatedDate).First();
                oldPoolItem.Dispose();
                _poolItens.Remove(oldPoolItem);
            }
        }

        private IPoolObject? GetExistingItem(string targetUrl)
            => _poolItens.FirstOrDefault(x => x.Page.Url.FormatUrl().SameUrl(targetUrl.FormatUrl()));

        private static bool IsOldItem(IPoolObject? existingPoolItem)
            => existingPoolItem != null && existingPoolItem.CreatedDate < DateTime.Now.AddMinutes(-ITEM_LIFE_TIME);

        private static bool IsRecentlyCreatedItem(IPoolObject? existingPoolItem)
            => existingPoolItem != null && existingPoolItem.CreatedDate > DateTime.Now.AddMinutes(-ITEM_LIFE_TIME);

        private void DisposeItem(IPoolObject existingPoolItem)
        {
            existingPoolItem.Dispose();
            _poolItens.Remove(existingPoolItem);
        }

        private async Task<IPoolObject> Create(string url)
        {
            var page = await _loader.LoadPage(url.FormatUrl());
            var poolObject = new PoolObject(_poolItens.Count + 1, DateTime.Now, page);
            _poolItens.Add(poolObject);
            return poolObject;
        }

        public void Dispose()
        {
            _poolItens?.ForEach(i => i.Dispose());
            semaphore?.Dispose();
        }
    }
}
