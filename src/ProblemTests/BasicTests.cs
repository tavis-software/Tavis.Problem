using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
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


        [Fact]
        public void RoundTripAProblem()
        {

            var problem = new ProblemDocument
            {
                ProblemType = new Uri("http://example.org"),
                Title = "Houston we have a problem",
                StatusCode = HttpStatusCode.BadGateway,
                ProblemInstance = new Uri("http://foo")
            };

            problem.Extensions.Add("bar",new JValue("100"));

            var ms = new MemoryStream();

            problem.Save(ms);

            ms.Position = 0;

            var problem2 = ProblemDocument.Parse(ms);
            
            Assert.Equal(problem, problem2);
            
        }

        


    }
}
