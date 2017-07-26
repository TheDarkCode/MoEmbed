using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoEmbed.Models
{
    public class LinkEmbedObject : EmbedObject
    {
        public LinkEmbedObject(string uri) : this(new Uri(uri))
        {
        }

        public LinkEmbedObject(Uri uri)
        {
        }

        /// <inheritdoc />
        public override Types Type => Types.Link;

        /// <summary>
        /// Gets or sets the requested URL.
        /// </summary>
        [DefaultValue(null)]
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the URL the <see cref="Uri" /> moved to.
        /// </summary>
        [DefaultValue(null)]
        public Uri MovedTo { get; set; }

        /// <inheritdoc />
        public async override Task FetchAsync()
        {
            using (var hh = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            })
            using (var hc = new HttpClient(hh))
            {
                // TODO: share HttpClient in service

                var res = await GetResponseAsync(hc).ConfigureAwait(false);
                if (!res.IsSuccessStatusCode)
                {
                    return;
                }

                var mediaType = res.Content.Headers.ContentType.MediaType;

                if (Regex.IsMatch(mediaType, @"^text\/html$"))
                {
                    LoadHtml(await res.Content.ReadAsStringAsync());
                }
                else if (Regex.IsMatch(mediaType, @"^image\/"))
                {
                }
                else if (Regex.IsMatch(mediaType, @"^video\/"))
                {
                }
                else
                {
                }
            }
        }

        private async Task<HttpResponseMessage> GetResponseAsync(HttpClient hc)
        {
            var u = MovedTo ?? Uri;
            for (;;)
            {
                var res = await hc.GetAsync(u).ConfigureAwait(false);

                switch (res.StatusCode)
                {
                    case HttpStatusCode.Moved:
                        if (u == (MovedTo ?? Uri))
                        {
                            MovedTo = res.Headers.Location;
                        }
                        u = res.Headers.Location;
                        continue;
                    case HttpStatusCode.Ambiguous:
                    case HttpStatusCode.Found:
                    case HttpStatusCode.RedirectMethod:
                        u = res.Headers.Location;
                        continue;
                }

                return res;
            }
        }

        private void LoadHtml(string html)
        {
            // TODO:this.Title = "fetched title";
        }
    }
}