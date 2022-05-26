
#if NET5_0 || NET6_0

using Our.Umbraco.EmbedHeaven.EmbedProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.EmbedHeaven
{
    public class EmbedHeavenComposer : IUserComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.OEmbedProviders().Append<SpotifyEmbedProvider>();
        }
    }
}

#endif
