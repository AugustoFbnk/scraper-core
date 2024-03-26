using Essentials.Web.Extensions;
using NSubstitute;
using PuppeteerSharp;
using Scraper.BackgroundTasks.Abstractions.Services.Browsering;
using Scraper.BackgroundTasks.Abstractions.Services.Pooling;
using Scraper.BackgroundTasks.Services.Pooling;

namespace Scraper.UnitTests.BackgroundTasks.Services
{
    [TestClass]
    public class BrowserPoolTest
    {
        [TestMethod]
        public async Task GetFreeObject_must_create_new_item_when_existing_is_too_old()
        {
            //Arrange
            var url = ("https://www.test_url.com").FormatUrl();
            var page = Substitute.For<IPage>();
            page.Url.Returns(url);

            var oldPoolObj = Substitute.For<IPoolObject>();
            oldPoolObj.CreatedDate.Returns(DateTime.Now.AddMinutes(-4));
            oldPoolObj.Page.Returns(page);

            var poolObjList = new List<IPoolObject>() { oldPoolObj };
            var loader = Substitute.For<IBrowserLoader>();
            var pool = new BrowserPool(loader, poolObjList);

            //Act
            var result = await pool.GetFreeObject(url);

            //Assert
            Assert.IsNotNull(result);
            await loader.Received(1).LoadPage(url);
        }

        [TestMethod]
        public async Task GetFreeObject_must_return_existing_when_isrecently_created()
        {
            //Arrange
            var url = ("https://www.test_url.com").FormatUrl();
            var page = Substitute.For<IPage>();
            page.Url.Returns(url);

            var recentlyCreatedPoolObj = Substitute.For<IPoolObject>();
            recentlyCreatedPoolObj.CreatedDate.Returns(DateTime.Now);
            recentlyCreatedPoolObj.Page.Returns(page);

            var poolObjList = new List<IPoolObject>() { recentlyCreatedPoolObj };
            var loader = Substitute.For<IBrowserLoader>();
            var pool = new BrowserPool(loader, poolObjList);

            //Act
            var result = await pool.GetFreeObject(url);

            //Assert
            Assert.AreEqual(recentlyCreatedPoolObj.GetHashCode(), result.GetHashCode());
            await loader.Received(0).LoadPage(url);
        }

        [TestMethod]
        public async Task GetFreeObject_must_create_new_item_when_not_exists_in_pool()
        {
            //Arrange
            var url = ("https://www.test_url.com").FormatUrl();
            var loader = Substitute.For<IBrowserLoader>();
            var pool = new BrowserPool(loader);

            //Act
            var result = await pool.GetFreeObject(url);

            //Assert
            Assert.IsNotNull(result);
            await loader.Received(1).LoadPage(url);
        }

        [TestMethod]
        public async Task GetFreeObject_must_recycle_pool_when_pool_size_exceed_maximun_allowed()
        {
            //Arrange
            var url = ("https://www.test_url.com").FormatUrl();

            var page = Substitute.For<IPage>();
            page.Url.Returns(url);

            var poolObj1 = Substitute.For<IPoolObject>();
            poolObj1.CreatedDate.Returns(DateTime.Now);
            poolObj1.Page.Returns(page);

            var poolObj2 = Substitute.For<IPoolObject>();
            poolObj2.CreatedDate.Returns(DateTime.Now);
            poolObj2.Page.Returns(page);

            var poolObj3 = Substitute.For<IPoolObject>();
            poolObj3.CreatedDate.Returns(DateTime.Now);
            poolObj3.Page.Returns(page);

            var poolObj4 = Substitute.For<IPoolObject>();
            poolObj4.CreatedDate.Returns(DateTime.Now);
            poolObj4.Page.Returns(page);

            var poolObj5 = Substitute.For<IPoolObject>();
            poolObj5.CreatedDate.Returns(DateTime.Now);
            poolObj5.Page.Returns(page);

            var olderPoolObj = Substitute.For<IPoolObject>();
            olderPoolObj.CreatedDate.Returns(DateTime.Now.AddMinutes(-5));
            olderPoolObj.Page.Returns(page);

            var poolObjList = new List<IPoolObject>()
            {
                poolObj1, poolObj2, poolObj3, poolObj4, poolObj5, olderPoolObj
            };

            var loader = Substitute.For<IBrowserLoader>();
            var pool = new BrowserPool(loader, poolObjList);

            //Act
            var result = await pool.GetFreeObject(url);

            //Assert
            Assert.IsFalse(poolObjList.Exists(x => x.GetHashCode() == olderPoolObj.GetHashCode()));
        }

        //Must dispose

        //Must not allow more than 1 access by time
    }
}
