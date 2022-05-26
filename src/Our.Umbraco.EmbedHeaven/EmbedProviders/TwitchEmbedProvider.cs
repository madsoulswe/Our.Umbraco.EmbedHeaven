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
	public class TwitchEmbedProvider : EmbedProviderBase
	{
#if NET5_0 || NET6_0
		public TwitchEmbedProvider(IJsonSerializer jsonSerializer) : base(jsonSerializer) { }
#endif
		public override string ApiEndpoint => "https://www.twitch.tv";

		public override string[] UrlSchemeRegex => new string[]
		{
			@"http[s]?:\/\/(?:www\.|clips\.)twitch\.tv\/([0-9a-zA-Z\-\_]+)\/?(chat\/?$|[0-9a-z\-\_]*)?"
		};

		public override Dictionary<string, string> RequestParams => new Dictionary<string, string>();

		public override string GetMarkup(string url, int maxWidth = 0, int maxHeight = 0)
		{
			return FakeResponse(url, maxWidth, maxHeight)?.GetHtml();
		}

		public OEmbedResponse FakeResponse(string url, int maxWidth = 0, int maxHeight = 0)
		{
			var providerUrl = "https://twitch.tv";
			var src = "";
			var regex = new Regex(UrlSchemeRegex[0], RegexOptions.IgnoreCase);
			var matches = regex.Match(url);

			if (matches.Success) {
				var channelName = matches.Groups[1];
				var type = getType(url);
				var attrs = "";

				// Clip, channel, chat, collection, or video?
				switch (type) {
	                case "clip":
	                    src = "https://clips.twitch.tv/embed?clip=" + channelName + "&autoplay=false";
	                    attrs = "scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\"";
					break;
	
	                case "video":
	                    channelName = matches.Groups[2];
	                    src = "https://player.twitch.tv/?video=" + channelName;
	                    attrs = "scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\"";
						break;
	
	                case "channel":
	                    src = "https://player.twitch.tv/?channel=" + channelName;
						attrs = "scrolling=\"no\" frameborder=\"0\" allowfullscreen=\"true\"";
						break;
	
	                case "chat":
	                    src = "http://www.twitch.tv/embed/" + channelName + "/chat";
						attrs = "scrolling=\"yes\" frameborder=\"0\" allowfullscreen=\"true\" id=\"" + channelName + "\"";
						break;
				}
				
	            //src = !empty($pars_url['host']) ?$src.'&parent='.$pars_url['host']:$src;
	            var html = $"<iframe src=\"{src}\" height=\"{(maxHeight == 0 ? 400 : maxHeight)}\" width=\"{(maxWidth == 0 ? 800 : maxWidth)}\"  {attrs}></iframe>";

				return new OEmbedResponse()
				{
					Type = type,
					Width = maxWidth,
					Height = maxHeight,
					Html = html,
					Url = url,
					ProviderName = "Twitch",
					ProviderUrl = providerUrl
				};
			}

			return null;
		}

		protected string getType(string url)
	    {
	        if (url.ToLower().Contains("clips.twitch.tv")) {
	            return "clip";
	        }
	
	        if (url.ToLower().Contains("/videos/")) {
				return "video";
	        }
	
	        if (Regex.IsMatch(@"#/chat$#", url)) {
				return "chat";
	        }
	
	        return "channel";
	    }
	}
}
