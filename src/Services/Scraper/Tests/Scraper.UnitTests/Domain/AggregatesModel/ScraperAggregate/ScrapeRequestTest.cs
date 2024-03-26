using Essentials;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.UnitTests.Domain.AggregatesModel.ScraperAggregate
{
    [TestClass]
    public class ScrapeRequestTest
    {
        [TestMethod]
        [DataRow("", 0)]//Arrange
        [DataRow("key1", 1)]
        [DataRow($"key1{SC.WORD_DELIMITER}key2", 2)]
        [DataRow($"key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}", 2)]
        [DataRow($"{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}key2", 2)]
        [DataRow($"{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}", 2)]
        [DataRow($"{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}", 2)]
        [DataRow($"{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}", 2)]
        [DataRow($"{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}{SC.WORD_DELIMITER}", 2)]
        [DataRow($"{SC.WORD_DELIMITER}key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}key3", 3)]
        public void GetSearchTextArray_must_cast_search_text_into_an_array_skiping_word_delimiter(string searchText, int expectedArrayCount)
        {
            //act
            var scrapeRequest = new ScrapeRequest(string.Empty, string.Empty, string.Empty, string.Empty)
            {
                SearchText = searchText
            };
            var searchTextArray = scrapeRequest.GetSearchTextArray();

            //Assert
            Assert.AreEqual(expectedArrayCount, searchTextArray.Length);
        }

        [TestMethod]
        [DynamicData(nameof(MatchPeersDataForGetMatchedPairTest))]//Arrange
        public void GetMatchedPair_must_filter_key_values_from_a_grouped_request(Dictionary<string, IEnumerable<string>> groupedRequests,
            Dictionary<string, IEnumerable<string>> expectedFounds,
            string searchText)
        {
            //Act
            var scrapeRequest = new ScrapeRequest(string.Empty, string.Empty, string.Empty, string.Empty)
            {
                SearchText = searchText
            };

            var actual = scrapeRequest.GetMatchedPair(groupedRequests);

            //Assert
            Assert.AreEqual(expectedFounds.Count, actual.Count);
            foreach (var foundWithKeysMatch in actual)
            {
                Assert.IsTrue(groupedRequests.ContainsKey(foundWithKeysMatch.Key));
            }
        }

        [TestMethod]
        public void RegisterUrlsFounds_must_add_founds_to_UrlFound_list()
        {
            //Arrange
            var foundsDictionary = new Dictionary<string, IEnumerable<string>>
            {
                { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key1"} },
                { "https://url_found_in_grouped_requests_founds_02", new List<string>{"key1", "key2"} },
                { "https://url_found_in_grouped_requests_founds_03", new List<string>{"key1", "key2", "key3"} }
            };
            var registeredFounds = new List<ScrapeUrlFound>()
            {
                new ScrapeUrlFound()
                {
                    KeyWorkds = "key1",
                    UrlFound = "https://url_found_in_grouped_requests_founds_01"
                },
                new ScrapeUrlFound()
                {
                    KeyWorkds = $"key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}",
                    UrlFound = "https://url_found_in_grouped_requests_founds_02"
                },
                new ScrapeUrlFound()
                {
                    KeyWorkds = $"key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}key3",
                    UrlFound = "https://url_found_in_grouped_requests_founds_03"
                },
            };

            //Act
            var scrapeRequest = new ScrapeRequest(string.Empty, string.Empty, string.Empty, string.Empty);
            scrapeRequest.RegisterUrlsFounds(foundsDictionary);

            //Assert
            Assert.IsTrue(scrapeRequest.UrlFound.Find(x => x.UrlFound == "https://url_found_in_grouped_requests_founds_01")?.KeyWorkds == "key1");
            Assert.IsTrue(scrapeRequest.UrlFound.Find(x => x.UrlFound == "https://url_found_in_grouped_requests_founds_02")?.KeyWorkds == $"key1{SC.WORD_DELIMITER}key2");
            Assert.IsTrue(scrapeRequest.UrlFound.Find(x => x.UrlFound == "https://url_found_in_grouped_requests_founds_03")?.KeyWorkds == $"key1{SC.WORD_DELIMITER}key2{SC.WORD_DELIMITER}key3");

        }

        public static IEnumerable<object[]> MatchPeersDataForGetMatchedPairTest
        {
            get
            {
                return new[]
                {
                    //Contains only found with key3
                     new object[]
                     {
                         new Dictionary<string, IEnumerable<string>>
                         {
                             { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key1"} },
                             { "https://url_found_in_grouped_requests_founds_02", new List<string>{"key1", "key2"} },
                             { "https://url_found_in_grouped_requests_founds_03", new List<string>{"key1", "key2", "key3"} }
                         },
                         new Dictionary<string, IEnumerable<string>>
                         {
                             { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key3"} }
                         },
                         $"key3{SC.WORD_DELIMITER}aleatoryKey1{SC.WORD_DELIMITER}aleatoryKey2"
                     },
                     
                     //Contains founds with key1 and key3
                     new object[]
                     {
                         new Dictionary<string, IEnumerable<string>>
                         {
                             { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key1"} },
                             { "https://url_found_in_grouped_requests_founds_02", new List<string>{"key1", "key2"} },
                             { "https://url_found_in_grouped_requests_founds_03", new List<string>{"key1", "key2", "key3"} }
                         },
                         new Dictionary<string, IEnumerable<string>>
                         {
                             { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key2"} },
                             { "https://url_found_in_grouped_requests_founds_02", new List<string>{"key3"} },
                         },
                         $"key2{SC.WORD_DELIMITER}key3{SC.WORD_DELIMITER}aleatoryKey1{SC.WORD_DELIMITER}aleatoryKey2"
                     },

                     //Do not contains founds
                     new object[]
                     {
                         new Dictionary<string, IEnumerable<string>>
                         {
                             { "https://url_found_in_grouped_requests_founds_01", new List<string>{"key1"} },
                             { "https://url_found_in_grouped_requests_founds_02", new List<string>{"key1", "key2"} },
                             { "https://url_found_in_grouped_requests_founds_03", new List<string>{"key1", "key2", "key3"} }
                         },
                         new Dictionary<string, IEnumerable<string>>(),
                         $"aleatoryKey1{SC.WORD_DELIMITER}aleatoryKey2"
                     },
                };
            }
        }
    }
}
