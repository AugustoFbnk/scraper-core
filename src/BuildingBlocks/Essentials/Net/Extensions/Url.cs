namespace Essentials.Web.Extensions
{
    public static class Url
    {
        public static bool SameUrl(this string urlSource, string urlTarget)
        {
            var inputUri = new Uri(urlTarget);
            var activeUri = new Uri(urlSource);
            return inputUri.IdnHost.ToLowerInvariant().RemoveTld() == activeUri.IdnHost.ToLowerInvariant().RemoveTld();
        }

        public static string FormatUrl(this string input) =>
            string.Concat(SC.HTTPS_SCHEME, input.ToLowerInvariant()
                .ValidateInputUrl()
                .RemoveScheme()
                .RemovePrefix()
                .NormalizeEnding()
                .ValidateOutputUrl());

        private static string ValidateInputUrl(this string input) =>
            input.StartsWith(SC.HTTP_SCHEME)
            ? throw new ArgumentException("A given URL is not safe!")
            : input;

        private static string ValidateOutputUrl(this string input) => (!Uri.IsWellFormedUriString(input, UriKind.Absolute))
                ? throw new ArgumentException("A given string is not a valid URL!")
                : input;

        private static string RemoveScheme(this string input) =>
            input.Replace(SC.HTTP_SCHEME, string.Empty)
                 .Replace(SC.HTTPS_SCHEME, string.Empty);

        private static string RemovePrefix(this string input) =>
            input.StartsWith(SC.WWW_PREFIX)
            ? input.Substring(SC.WWW_PREFIX.Length)
            : input;

        private static string RemoveTld(this string input) =>
            input.RemoveTld(SC.TLD_BR)
                 .RemoveTld(SC.TLD_US)
                 .RemoveTld(SC.TLD_UK)
                 .RemoveTld(SC.TLD_CA)
                 .RemoveTld(SC.TLD_AU)
                 .RemoveTld(SC.TLD_JP)
                 .RemoveTld(SC.TLD_DE)
                 .RemoveTld(SC.TLD_FR)
                 .RemoveTld(SC.TLD_CN);

        private static string RemoveTld(this string input, string tld) =>
            input.EndsWith(tld)
            ? input.Substring(0, input.Length - tld.Length)
            : input;

        private static string NormalizeEnding(this string input) =>
            input.EndsWith(SC.URL_ENDING)
            ? input
            : string.Concat(input, SC.URL_ENDING);
    }
}
