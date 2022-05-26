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
	public class GoogleDocsEmbedProvider : EmbedProviderBase
	{
#if NET5_0 || NET6_0
		public GoogleDocsEmbedProvider(IJsonSerializer jsonSerializer) : base(jsonSerializer) { }
#endif
		public override string ApiEndpoint => "https://docs.google.com/";

		public override string[] UrlSchemeRegex => new string[]
		{
			@"http[s]?:\/\/((?:www\.)?docs\.google\.com\/(?:.*\/)?(?:document|presentation|spreadsheets|forms|drawings)\/[a-z0-9\/\?=_\-\.\,&%\$#\@\!\+]*)"
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

			var typeRegex = Regex.Match(url, @"google\.com\/(?:.+\/)?(document|presentation|spreadsheets|forms|drawings)/");
			var type = typeRegex.Groups[1]?.ToString().ToLower();

			switch (type)
            {
				case "document":
					if (!Regex.IsMatch(src, @"([?&])embedded=true")) {
	                    if (src.IndexOf('?') > -1) {
                          src += "&embedded=true";
	                    } else {
                          src += "?embedded=true";
	                    }
	                }
					break;

				case "presentation":
					if (!Regex.IsMatch(src, @"/pub\?"))
					{
						src = src.Replace("/pub?", "/embed?");
					}
					break;

				case "spreadsheets":
					if (src.IndexOf('?') > -1)
					{
						if (src.IndexOf("widget=", StringComparison.InvariantCultureIgnoreCase) == -1)
							src += "&widget=true";

						if (src.IndexOf("headers=", StringComparison.InvariantCultureIgnoreCase) == -1)
							src += "&headers=false";
					} 
					else
                    {
						src += "?widget=true&headers=false";
					}
					break;

				case "forms":
				case "drawings":
					break;
			}
			var html = "";
			//src = !empty($pars_url['host']) ?$src.'&parent='.$pars_url['host']:$src;

			if(type != "drawing")
            {
				html = $"<iframe src=\"{src}\" frameborder=\"0\" height=\"{(maxHeight == 0 ? 450 : maxHeight)}\" width=\"{(maxWidth == 0 ? 600 : maxWidth)}\"  allowfullscreen=\"true\" mozallowfullscreen=\"true\" webkitallowfullscreen=\"true\" { attrs}></iframe>";
			} else
            {
				html = $"<iframe src=\"{src}\" frameborder=\"0\" height=\"{(maxHeight == 0 ? 720 : maxHeight)}\" width=\"{(maxWidth == 0 ? 960 : maxWidth)}\"  {attrs}></iframe>";
			}
			

			return new OEmbedResponse()
			{
				Type = "rich",
				Width = maxWidth,
				Height = maxHeight,
				Html = html,
				Url = url,
				ProviderName = "Google Docs",
				ProviderUrl = "https://docs.google.com"
			};
			
		}

	}
}
