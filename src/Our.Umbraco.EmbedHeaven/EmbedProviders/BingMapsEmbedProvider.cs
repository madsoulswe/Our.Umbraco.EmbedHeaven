using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

#if NET472
using Umbraco.Web.Media.EmbedProviders;
#elif NET5_0 || NET6_0
using Umbraco.Cms.Core.Media.EmbedProviders;
using Umbraco.Cms.Core.Serialization;
#endif


namespace Our.Umbraco.EmbedHeaven.EmbedProviders
{
    public class BingMapsEmbedProvider : EmbedProviderBase
    {
#if NET5_0 || NET6_0
        public GoogleMapsEmbedProvider(IJsonSerializer jsonSerializer) : base(jsonSerializer) { }
#endif
        public override string ApiEndpoint => "https://maps.google.com/";

        public override string[] UrlSchemeRegex => new string[]
        {
            @"http[s]?:\/\/(?:(?:(?:www\.)?(?:bing\.com?)))\/(?:maps\/)"
        };

        public override Dictionary<string, string> RequestParams => new Dictionary<string, string>();

        public override string GetMarkup(string url, int maxWidth = 0, int maxHeight = 0)
        {
            return FakeResponse(url, maxWidth, maxHeight)?.GetHtml();
        }

        public OEmbedResponse FakeResponse(string url, int maxWidth = 0, int maxHeight = 0)
        {
            var src = "";
            var height = (maxHeight == 0 ? 450 : maxHeight);
            var width = (maxWidth == 0 ? 600 : maxWidth);

            if (Regex.IsMatch(url, "(maps/embed)", RegexOptions.IgnoreCase))
            {
                src = url;
            }
            else
            {
                var query = HttpUtility.ParseQueryString(url);

                if (query["cp"] == null)
                    return null;

                src = $"https://www.bing.com/maps/embed?h={height}&w={width}&cp={query["cp"]}&lvl={(query["lvl"] ?? "1")}& typ=d&sty=r&src=SHELL&FORM=MBEDV8";
            }

            var html = $"<iframe src=\"{src}\" height=\"{height}\" width=\"{width}\" scrolling=\"no\"></iframe>";

            return new OEmbedResponse()
            {
                Type = "rich",
                Width = width,
                Height = height,
                Html = html,
                Url = url,
                ProviderName = "Bing Maps",
                ProviderUrl = "https://www.bing.com/maps/"
            };
            
        }

    }
}
