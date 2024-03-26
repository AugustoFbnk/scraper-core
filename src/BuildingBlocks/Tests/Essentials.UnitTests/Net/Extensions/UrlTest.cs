using Essentials.Web.Extensions;

namespace Essentials.UnitTests.Net.Extensions
{
    [TestClass]
    public class UrlTest
    {
        [TestMethod]
        public void SameUrl_must_return_true_when_is_same_url()
        {
            var sourceUrl = "http://www.test.com";
            var targetUrl = "http://www.test.com.br";

            Assert.IsTrue(sourceUrl.SameUrl(targetUrl));
        }

        //must not accept scheme missing (http, https)

    }
}
