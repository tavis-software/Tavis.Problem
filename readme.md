# Tavis.Problem

This library provides .Net support for the media type `application/api-problem+json` http://tools.ietf.org/html/draft-nottingham-http-problem-04




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