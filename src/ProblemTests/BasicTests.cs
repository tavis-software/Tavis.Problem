using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Tavis;
using Xunit;

namespace ProblemTests
{
    public class BasicTests
    {
        [Fact]
        public void CreateAProblem()
        {

            var problem = new ProblemDocument
            {
                ProblemType = new Uri("http://example.org"),
                Title = "Houston we have a problem",
                StatusCode = HttpStatusCode.BadGateway,
                ProblemInstance = new Uri("http://foo")
            };

            Assert.NotNull(problem);

        }
    }
}
