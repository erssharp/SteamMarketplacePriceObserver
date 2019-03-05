using AngleSharp.Dom.Html;

namespace SteamMarketplacePriceObserver.Core
{
    interface IParser<T> where T : class
    {
        T Parse(IHtmlDocument document);
    }
}
