using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    public class ProblemDocument
    {
        public Uri ProblemType { get; set; }
        public string Title { get; set; }

        public HttpStatusCode? StatusCode { get; set; }
        public string Detail { get; set; }
        public Uri ProblemInstance { get; set; }

        public Dictionary<string, JToken> Extensions { get; set; }

        public ProblemDocument()
        {
            Extensions = new Dictionary<string, JToken>();
        }

        public void Save(System.IO.MemoryStream stream)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var jsonWriter = new JsonTextWriter(sw) {Formatting = Formatting.Indented};

            WriteProblem(jsonWriter);

            var stw = new StreamWriter(stream);
            stw.Write(sb.ToString());
            stw.Flush();
            jsonWriter.Close();

        }

        private void WriteProblem(JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();

            WriteProperty(jsonWriter, "problemType", ProblemType.OriginalString);
            WriteProperty(jsonWriter, "title", Title);

            if (StatusCode != null)
            {
                jsonWriter.WritePropertyName("httpStatus");
                jsonWriter.WriteValue((int)StatusCode);
            }

            if (!String.IsNullOrEmpty(Detail))
            {
                WriteProperty(jsonWriter, "detail", Detail);
            }

            if (ProblemInstance != null)
            {
                WriteProperty(jsonWriter, "problemInstance", ProblemInstance.OriginalString);
            }

            foreach (var extension in Extensions)
            {
                jsonWriter.WritePropertyName(extension.Key);
                extension.Value.WriteTo(jsonWriter);
            }

            jsonWriter.WriteEndObject();
        }

        private void WriteProperty(JsonWriter jsonWriter, string name, string value)
        {
            jsonWriter.WritePropertyName(name);
            jsonWriter.WriteValue(value);
        }

        public static ProblemDocument Parse(Stream jsonStream)
        {
            var sr = new StreamReader(jsonStream);
            return Parse(sr.ReadToEnd());
        }

          public static ProblemDocument Parse(string jsonString)
        {
            var jDoc = JObject.Parse(jsonString);
            return Parse(jDoc);
        }


        public static ProblemDocument Parse(JObject jObject)
        {
            var doc = new ProblemDocument();


            foreach (var jProp in jObject.Properties()){

                switch (jProp.Name)
                {
                    case "problemType":
                        doc.ProblemType = new Uri((string)jProp.Value, UriKind.RelativeOrAbsolute);
                        break;
                    case "title":
                        doc.Title = (string)jProp.Value;
                        break;
                    case "httpStatus":
                        doc.StatusCode = (HttpStatusCode)(int)jProp.Value;
                        break;
                    case "detail":
                        doc.Detail = (string)jProp.Value;
                        break;
                    case "problemInstance":
                        doc.ProblemInstance = new Uri((string) jProp.Value);
                        break;
                    default:
                        doc.Extensions.Add(jProp.Name,jProp.Value);                
                        break;
                }
    
            }

            if (doc.ProblemType == null) throw new ArgumentException("Missing problemType property");
            if (string.IsNullOrEmpty(doc.Title)) throw new ArgumentException("Missing title property");

            return doc;
        }

        public override bool Equals(object obj)
        {
            var newProblem = (ProblemDocument) obj;
            var equal = (newProblem.ProblemType.OriginalString == this.ProblemType.OriginalString) &&
                        (newProblem.Title == Title) &&
                        (newProblem.Detail == Detail) &&
                        (newProblem.StatusCode == StatusCode) &&
                        (newProblem.ProblemInstance.OriginalString == ProblemInstance.OriginalString) &&
                        newProblem.Extensions.SequenceEqual(Extensions);

            
            return equal;
        }
    }

    
}
