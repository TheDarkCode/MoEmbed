using System.Collections.Generic;
using System.Text.RegularExpressions;
using MoEmbed.Models;
using MoEmbed.Models.Metadata;

namespace MoEmbed.Providers
{
    /// <summary>
    /// Represents the <see cref="Metadata"/> for the URL of the imgur.com
    /// </summary>
    public class ImgurMetadataProvider : UnknownMetadataProvider, IMetadataProvider
    {
        bool IMetadataProvider.SupportsAnyHost
            => false;

        private static readonly Regex regex = new Regex(@"^https?://imgur\.com/[a-zA-Z0-9]+$");

        /// <summary>
        /// Returns a sequence of host names that is able to handle.
        /// </summary>
        /// <returns>The sequence of host names.</returns>
        public override IEnumerable<string> GetSupportedHostNames()
            => new[] { "imgur.com" };

        public override bool CanHandle(ConsumerRequest request)
            => regex.IsMatch(request.Url.ToString());

        public override Metadata GetMetadata(ConsumerRequest request)
        {
            if (!CanHandle(request))
            {
                return null;
            }
            return new ImgurMetadata()
            {
                Uri = request.Url.ToString(),
            };
        }
    }
}