using Scraper.Domain.Dto;

namespace Scraper.Domain.Extensions
{
    public static class ScrapingRequestExtensions
    {
        public static IEnumerable<string> GetIntersectMatches(this GroupedRequest groupedRequest, ScrapePair? scrapeItem)
        {
            var matchesFoundInText = groupedRequest.SearchTexts.Intersect(scrapeItem?.Text?.Split() ?? new string[] { }, StringComparer.OrdinalIgnoreCase);
            var matchesFoundInUrl = groupedRequest.SearchTexts.Intersect(scrapeItem?.Url?.Split() ?? new string[] { }, StringComparer.OrdinalIgnoreCase);
            var matches = matchesFoundInText.Concat(matchesFoundInUrl).Distinct();
            return matches;
        }

        public static IEnumerable<string> GetContainingMatches(this GroupedRequest groupedRequest, ScrapePair? scrapeItem)
        {
            var text = scrapeItem?.Text?.Split().ToList() ?? new List<string>();
            var url = scrapeItem?.Url?.Split().ToList() ?? new List<string>();

            var mathcesInText = groupedRequest.SearchTexts.Where(st => text.Any(text => text.Contains(st)));
            var mathcesInUrl = groupedRequest.SearchTexts.Where(st => url.Any(text => text.Contains(st)));
            var matches = mathcesInText.Concat(mathcesInUrl).Distinct();
            return matches;
        }
    }
}
