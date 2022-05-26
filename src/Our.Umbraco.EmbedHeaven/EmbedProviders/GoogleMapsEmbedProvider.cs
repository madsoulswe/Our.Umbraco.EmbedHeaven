using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#if NET472
using Umbraco.Web.Media.EmbedProviders;
#elif NET5_0 || NET6_0
using Umbraco.Cms.Core.Media.EmbedProviders;
using Umbraco.Cms.Core.Serialization;
#endif


namespace Our.Umbraco.EmbedHeaven.EmbedProviders
{
	public class GoogleMapsEmbedProvider : EmbedProviderBase
	{
#if NET5_0 || NET6_0
		public GoogleMapsEmbedProvider(IJsonSerializer jsonSerializer) : base(jsonSerializer) { }
#endif
		public override string ApiEndpoint => "https://maps.google.com/";

		public override string[] UrlSchemeRegex => new string[]
		{
			@"http[s]?:\/\/(?:(?:(?:www\.|maps\.)?(?:google\.com?))|(?:goo\.gl))(?:\.[a-z]{2})?\/(?:maps\/)?(?:place\/)?(?:[a-z0-9\/%+\-_]*)?([a-z0-9\/%,+\-_=!:@\.&*\$#?\']*)"
		};

		public override Dictionary<string, string> RequestParams => new Dictionary<string, string>();

		public override string GetMarkup(string url, int maxWidth = 0, int maxHeight = 0)
		{
			return FakeResponse(url, maxWidth, maxHeight)?.GetHtml();
		}

		public OEmbedResponse FakeResponse(string url, int maxWidth = 0, int maxHeight = 0)
		{
			var src = "";

			var attrs = "";

			if(Regex.IsMatch(url, "(maps/embed|output=embed)", RegexOptions.IgnoreCase))
            {
				src = url;
            }
			else
            {
				var cordRegex = new Regex(@"@(-?[0-9\.]+,-?[0-9\.]+).+,([0-9\.]+[a-z])", RegexOptions.IgnoreCase);
				var matches = cordRegex.Match(url);

				if(matches.Success)
				{
					src = $"https://maps.google.com/maps?hl=en&ie=UTF8&ll={matches.Groups[1]}&spn={matches.Groups[1]}&t=m&z={matches.Groups[2]}&output=embed";
				}
            }

	        //src = !empty($pars_url['host']) ?$src.'&parent='.$pars_url['host']:$src;
	        var html = $"<iframe src=\"{src}\" height=\"{(maxHeight == 0 ? 450 : maxHeight)}\" width=\"{(maxWidth == 0 ? 600 : maxWidth)}\"  {attrs}></iframe>";

			return new OEmbedResponse()
			{
				Type = "rich",
				Width = maxWidth,
				Height = maxHeight,
				Html = html,
				Url = url,
				ProviderName = "Google Maps",
				ProviderUrl = "https://maps.google.com"
			};
			
		}

	}
}
