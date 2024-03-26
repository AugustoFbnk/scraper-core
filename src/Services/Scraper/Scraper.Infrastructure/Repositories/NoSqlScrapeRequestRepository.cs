using Essentials;
using Google.Cloud.Firestore;
using Scraper.Domain.AggregatesModel.ScraperAggregate;

namespace Scraper.Infrastructure.Repositories
{
    //--------------DRAFT!!!--------------
    public class NoSqlScrapeRequestRepository : IScrapeRequestRepository
    {
        private readonly FirestoreDb _context;

        public NoSqlScrapeRequestRepository(FirestoreDb context)
        {
            _context = context;
        }

        public async Task DeleteByIdAsync(long id, string userId)
        {
            var collection = _context.Collection("scrape_request");
            var query = collection.WhereEqualTo("Id", id);
            var querySnapshot = await query.GetSnapshotAsync();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                await documentSnapshot.Reference.DeleteAsync();
            }

            var collection2 = _context.Collection("scrape_url_found");
            var query2 = collection2.WhereEqualTo("ScrapeRequestId", id);
            var querySnapshot2 = await query2.GetSnapshotAsync();
            foreach (var documentSnapshot2 in querySnapshot2.Documents)
            {
                await documentSnapshot2.Reference.DeleteAsync();
            }
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsCascadeAsync()
        {
            var collection = _context.Collection("scrape_request");
            var querySnapshot = await collection.GetSnapshotAsync();

            var list = new List<ScrapeRequest>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var keyworkds = documentSnapshot.GetValue<string>("SearchText")
                  ?.Split(SC.WORD_DELIMITER)
                  ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                  ?.ToArray();

                var founds = new List<ScrapeUrlFound>();

                var collectionUrlFound = _context.Collection("scrape_url_found");
                var queryUrlFound = collectionUrlFound.WhereEqualTo("ScrapeRequestId", documentSnapshot.GetValue<string>("Id"));
                var querySnapshotUrlFound = await queryUrlFound.GetSnapshotAsync();
                foreach (var docQuerySnapshot in querySnapshotUrlFound.Documents)
                {
                    var keysFound = docQuerySnapshot.GetValue<string>("SearchText")
                      ?.Split(SC.WORD_DELIMITER)
                      ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                      ?.ToArray();
                    founds.Add(new ScrapeUrlFound()
                    {
                        ScrapeRequestId = docQuerySnapshot.GetValue<string>("ScrapeRequestId"),
                        UrlFound = docQuerySnapshot.GetValue<string>("UrlFound"),
                        KeyWorkds = docQuerySnapshot.GetValue<string>("KeyWorkds")
                    });
                }

                list.Add(new ScrapeRequest(documentSnapshot.GetValue<string>("UserId"),
                    documentSnapshot.GetValue<string>("Url"),
                    documentSnapshot.GetValue<string>("SearchText"),
                    documentSnapshot.GetValue<string>("PushNotificationToken")));
            }
            return list;

        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetAllRequestsAsync()
        {
            var collection = _context.Collection("scrape_request");
            var querySnapshot = await collection.GetSnapshotAsync();

            var list = new List<ScrapeRequest>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var keyworkds = documentSnapshot.GetValue<string>("SearchText")
                  ?.Split(SC.WORD_DELIMITER)
                  ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                  ?.ToArray();

                var request = new ScrapeRequest(documentSnapshot.GetValue<string>("UserId"),
                    documentSnapshot.GetValue<string>("Url"),
                    documentSnapshot.GetValue<string>("SearchText"),
                    documentSnapshot.GetValue<string>("PushNotificationToken"));
                request.Id = documentSnapshot.GetValue<long>("Id");

                list.Add(request);
            }
            return list;
        }

        public async Task<IReadOnlyList<ScrapeRequest>> GetByIdAsync(long id)
        {
            var collection = _context.Collection("scrape_request");
            var query = collection.WhereEqualTo("Id", id);
            var querySnapshot = await query.GetSnapshotAsync();

            var list = new List<ScrapeRequest>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var keyworkds = documentSnapshot.GetValue<string>("SearchText")
                  ?.Split(SC.WORD_DELIMITER)
                  ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                  ?.ToArray();

                var founds = new List<ScrapeUrlFound>();

                var collectionUrlFound = _context.Collection("scrape_url_found");
                var queryUrlFound = collectionUrlFound.WhereEqualTo("ScrapeRequestId", documentSnapshot.GetValue<string>("Id"));
                var querySnapshotUrlFound = await queryUrlFound.GetSnapshotAsync();
                foreach (var docQuerySnapshot in querySnapshotUrlFound.Documents)
                {
                    var keysFound = docQuerySnapshot.GetValue<string>("KeyWorkds")
                      ?.Split(SC.WORD_DELIMITER)
                      ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                      ?.ToArray();

                    founds.Add(new ScrapeUrlFound()
                    {
                        Id = new Random().NextInt64(1, 1000000), //Guid.NewGuid().ToString(),
                        KeyWorkds = docQuerySnapshot.GetValue<string>("KeyWorkds"),
                        ScrapeRequestId = docQuerySnapshot.GetValue<string>("ScrapeRequestId"),
                        UrlFound = docQuerySnapshot.GetValue<string>("UrlFound")
                    });
                }

                var scrapeRequest = new ScrapeRequest(documentSnapshot.GetValue<string>("UserId"),
                    documentSnapshot.GetValue<string>("Url"),
                    documentSnapshot.GetValue<string>("SearchText"),
                    documentSnapshot.GetValue<string>("PushNotificationToken"));
                scrapeRequest.UrlFound = founds ?? new List<ScrapeUrlFound>();
                list.Add(scrapeRequest);
            }

            return list;
        }

        public async Task CreateRequestAsync(ScrapeRequest scrape)
        {
            var collection = _context.Collection("scrape_request");
            _ = await collection.AddAsync(new
            {
                Id = new Random().NextInt64(1, 1000000), //Guid.NewGuid().ToString(),
                Url = scrape.Url,
                SearchText = scrape.SearchText,
                PushNotificationToken = scrape.PushNotificationToken,
                UserId = scrape.UserId,
            });
        }

        public async Task UpdateRequestAsync(long id, string userId, string searchText)
        {
            var collection = _context.Collection("scrape_request");
            var query = collection.WhereEqualTo("Id", id);
            var querySnapshot = await query.GetSnapshotAsync();

            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var updateData = new Dictionary<string, object>
                {
                    { "SearchText", searchText}
                };
                var documentReference = collection.Document(documentSnapshot.Id);

                await documentReference.UpdateAsync(updateData);

            }
        }

        public async Task UpdateRequestAsync(ScrapeRequest scrape)
        {
            var collection = _context.Collection("scrape_url_found");
            var query = collection.WhereEqualTo("ScrapeRequestId", scrape.Id);
            var querySnapshot = await query.GetSnapshotAsync();

            var urls = new List<string>();
            foreach (var found in scrape.UrlFound)
            {
                if (querySnapshot.Documents.Where(x => x.TryGetValue<string>("UrlFound", out string value) && value != found.UrlFound).ToList().Count > 0)
                {
                    continue;
                }

                _ = await collection.AddAsync(new
                {
                    Id = new Random().NextInt64(1, 1000000), //Guid.NewGuid().ToString(),
                    ScrapeRequestId = scrape.Id,
                    UrlFound = found.UrlFound,
                    KeyWorkds = found.KeyWorkds
                });
            }

        }
    }
}
