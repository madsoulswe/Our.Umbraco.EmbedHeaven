using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET472
using Umbraco.Web.Media.EmbedProviders;
#elif NET5_0 || NET6_0
using Umbraco.Cms.Core.Media.EmbedProviders;
using Umbraco.Cms.Core.Serialization;
#endif


namespace Our.Umbraco.EmbedHeaven.EmbedProviders
{
    public class SpotifyEmbedProvider : EmbedProviderBase
    {
    #if NET5_0 || NET6_0
        public SpotifyEmbedProvider(IJsonSerializer jsonSerializer) : base(jsonSerializer) { }
    #endif
        public override string ApiEndpoint => "https://open.spotify.com/oembed?url=";

        public override string[] UrlSchemeRegex => new string[]
        {
            @"open.spotify.com/*"
        };

        public override Dictionary<string, string> RequestParams => new Dictionary<string, string>();

        public override string GetMarkup(string url, int maxWidth = 0, int maxHeight = 0)
        {
            var requestUrl = base.GetEmbedProviderUrl(url, maxWidth, maxHeight);
            var oembed = base.GetJsonResponse<OEmbedResponse>(requestUrl);

            return oembed?.GetHtml();
        }
    }
}
