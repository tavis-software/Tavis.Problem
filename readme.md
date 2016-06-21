# Tavis.Problem

This library provides .Net support for the media type `application/problem+json` defined in [RFC 7807](https://tools.ietf.org/html/rfc7807)




		var problem = new ProblemDocument
            {
                ProblemType = new Uri("http://example.org"),
                Title = "Houston we have a problem",
                StatusCode = HttpStatusCode.BadGateway,
                ProblemInstance = new Uri("http://foo")
            };

            problem.Extensions.Add("bar", new JValue("100"));

        var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new ProblemContent(problem)
            };
