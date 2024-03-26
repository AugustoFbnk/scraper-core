namespace Scraper.BackgroundTasks
{
    public static class XPath
    {
        public const string GET_TEXT_AND_HREF_EXPRESSION = @"
function getElementsByXPath(xpath, parent)
{
    let resultsArray = [];
    let query = document.evaluate(xpath, parent || document,
        null, XPathResult.ORDERED_NODE_SNAPSHOT_TYPE, null);

    for (let i = 0, length = query.snapshotLength; i < length; ++i) {
        resultsArray.push({text: query.snapshotItem(i).textContent, url: query.snapshotItem(i).getAttribute('href')} );
    }
    return resultsArray.map( t=> {return { text: t.text, url: t.url}});
}
getElementsByXPath(""//a""); ";

    }
}
