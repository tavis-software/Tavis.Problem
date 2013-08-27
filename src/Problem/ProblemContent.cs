using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tavis
{
    public class ProblemContent : HttpContent
    {
        private readonly MemoryStream _problemStream;
        // Outbound
        public ProblemContent(ProblemDocument problemDocument)
        {
            // Problem documents tend to be small so we should serialize them immediately so that we can return the length of the stream.
            // This should prevent the host from inadverently chunking the response because it doesn't know the size of the response. 
            _problemStream = new MemoryStream();
            problemDocument.Save(_problemStream);
            _problemStream.Position = 0;
        }


        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            _problemStream.CopyTo(stream);  // Because this is likely a small document there is no advantage to switching threads to do this copy.
            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }

        protected override bool TryComputeLength(out long length)
        {
            throw new NotImplementedException();
        }

    
    }


    public static class HttpContentProblemExtensions
    {
            public static Task<ProblemDocument> ReadAsProblemAsync(this HttpContent content)
            {
                if (content.Headers.ContentType.MediaType.ToLowerInvariant() != "application/api-problem+json")
                {
                    throw new ArgumentException("Cannot process HttpContent with media type " + content.Headers.ContentType.MediaType);
                }

                return content.ReadAsStreamAsync().ContinueWith(t => ProblemDocument.Parse(t.Result));
            }
    }
}
