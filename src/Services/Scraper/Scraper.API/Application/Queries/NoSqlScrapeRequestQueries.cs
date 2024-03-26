using Essentials;
using Google.Cloud.Firestore;
using Scraper.API.Application.Models;

namespace Scraper.API.Application.Queries
{
    public class NoSqlScrapeRequestQueries : IScrapeRequestQueries
    {
        private readonly FirestoreDb _context;

        public NoSqlScrapeRequestQueries(FirestoreDb context)
        {
            _context = context;
        }

        public async Task<List<ScrapeRequestModel>> GetByUserAsync(string userId)
        {
            var collection = _context.Collection("scrape_request");
            var query = collection.WhereEqualTo("UserId", userId);
            var querySnapshot = await query.GetSnapshotAsync();

            var list = new List<ScrapeRequestModel>();
            foreach (var documentSnapshot in querySnapshot.Documents)
            {
                var keyworkds = documentSnapshot.GetValue<string>("SearchText")
                  ?.Split(SC.WORD_DELIMITER)
                  ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                  ?.ToArray();

                var founds = new List<ScrapeUrlFoundModel>();

                var collectionUrlFound = _context.Collection("scrape_url_found");
                var queryUrlFound = collectionUrlFound.WhereEqualTo("ScrapeRequestId", documentSnapshot.GetValue<string>("Id"));
                var querySnapshotUrlFound = await queryUrlFound.GetSnapshotAsync();
                foreach (var docQuerySnapshot in querySnapshotUrlFound.Documents)
                {
                    var keysFound = docQuerySnapshot.GetValue<string>("KeyWorkds")
                      ?.Split(SC.WORD_DELIMITER)
                      ?.Where(keyWord => !string.IsNullOrWhiteSpace(keyWord))
                      ?.ToArray();
                    founds.Add(new ScrapeUrlFoundModel(docQuerySnapshot.GetValue<string>("UrlFound"), keysFound ?? new string[] { }));
                }

                list.Add(new ScrapeRequestModel(documentSnapshot.GetValue<string>("Id"),
                    documentSnapshot.GetValue<string>("UserId"),
                    documentSnapshot.GetValue<string>("Url"),
                    keyworkds ?? new string[] { },
                    founds));
            }

            return list;
        }

        private List<ScrapeRequestModel> MapScrapeRequest(string userId, IEnumerable<dynamic> result)
        {
            return result
                .GroupBy(x => new { x.Id, x.SearchText, x.Url })
                .Select(item => new ScrapeRequestModel(item.Key.Id,
                                                       userId,
                                                       item.Key.Url,
                                                       GetKeysList(item.Key.SearchText),
                                                       GetUrlsFound(result.Where(x => x.Id == item.Key.Id).ToList())))
                .ToList();
        }

        private List<ScrapeUrlFoundModel> GetUrlsFound(List<dynamic> urlsFound)
        {
            return (from item in urlsFound
                    where item.UrlFound != null && item.KeyWords != null
                    select new ScrapeUrlFoundModel(item.UrlFound.ToString(), GetKeysList(item?.KeyWords)))?.ToList() ?? new List<ScrapeUrlFoundModel>();
        }

        private static string[] GetKeysList(dynamic searchText) => (searchText as string)?.Split(SC.WORD_DELIMITER).ToArray() ?? Array.Empty<string>();
    }
}
