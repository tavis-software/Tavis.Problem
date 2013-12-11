using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Tavis
{
    /// <summary>
    /// Extension methods to support reading ProblemDocument intstances from HttpContent objects
    /// </summary>
    public static class HttpContentProblemExtensions
    {

        /// <summary>
        /// Reading ProblemDocument intstance from HttpContent
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Task<ProblemDocument> ReadAsProblemAsync(this HttpContent content)
        {
            if (content.Headers.ContentType.MediaType.ToLowerInvariant() != "application/problem+json")
            {
                throw new ArgumentException("Cannot process HttpContent with media type " + content.Headers.ContentType.MediaType);
            }

            return content.ReadAsStreamAsync().ContinueWith(t => ProblemDocument.Parse(t.Result));
        }
    }
}