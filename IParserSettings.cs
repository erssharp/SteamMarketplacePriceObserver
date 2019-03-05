
namespace SteamMarketplacePriceObserver.Core
{
    interface IParserSettings
    {
        string BaseUrl { get; set; }

        string Prefix { get; set; }
    }
}
