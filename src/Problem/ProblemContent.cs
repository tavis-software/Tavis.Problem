using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tavis
{
    /// <summary>
    /// Convert ProblemDocument instance into a HttpContent object for sending across the wire
    /// </summary>
    public class ProblemContent : HttpContent
    {
        private readonly MemoryStream _problemStream;
        

        /// <summary>
        /// Create a instance of a ProblemContent object from a ProblemDocument
        /// </summary>
        /// <param name="problemDocument"></param>
        public ProblemContent(ProblemDocument problemDocument)
        {
            // Problem documents tend to be small so we should serialize them immediately so that we can return the length of the stream.
            // This should prevent the host from inadverently chunking the response because it doesn't know the size of the response. 
            _problemStream = new MemoryStream();
            problemDocument.Save(_problemStream);
            _problemStream.Position = 0;
            Headers.ContentType = new MediaTypeHeaderValue("application/problem+json");
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
            length = _problemStream.Length;
            return true;
        }

    
    }
}
