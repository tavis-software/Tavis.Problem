using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class Problem
    {
        public Uri ProblemType { get; set; }
        public string Title { get; set; }

        public HttpStatusCode StatusCode { get; set; }
        public string Detail { get; set; }
        public Uri ProblemInstance { get; set; }

        public Dictionary<string, JToken> Extensions { get; set; } 
    }
}
