using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tavis
{
    /// <summary>
    /// Object model for application/problem+json document
    /// </summary>
    public class ProblemDocument
    {
        public Uri ProblemType { get; set; }
        public string Title { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public string Detail { get; set; }
        public Uri ProblemInstance { get; set; }
        public Dictionary<string, JToken> Extensions { get; set; }

        /// <summary>
        /// Create a new problem documents
        /// </summary>
        public ProblemDocument()
        {
            Extensions = new Dictionary<string, JToken>();
        }

        /// <summary>
        /// Serialize current problem document as JSON representation
        /// </summary>
        /// <param name="stream"></param>
        public void Save(System.IO.Stream stream)
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

        /// <summary>
        /// Serialize current problem document as JSON representation
        /// </summary>
        /// <param name="stream"></param>
        public async Task SaveAsync(System.IO.Stream stream)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var jsonWriter = new JsonTextWriter(sw) {Formatting = Formatting.Indented};

            WriteProblem(jsonWriter);

            var stw = new StreamWriter(stream);
            await stw.WriteAsync(sb.ToString());
            stw.Flush();
            jsonWriter.Close();
        }

        private void WriteProblem(JsonWriter jsonWriter)
        {
            jsonWriter.WriteStartObject();

            WriteProperty(jsonWriter, "type", ProblemType.OriginalString);
            WriteProperty(jsonWriter, "title", Title);

            if (StatusCode != null)
            {
                jsonWriter.WritePropertyName("status");
                jsonWriter.WriteValue((int)StatusCode);
            }

            if (!string.IsNullOrEmpty(Detail))
            {
                WriteProperty(jsonWriter, "detail", Detail);
            }

            if (ProblemInstance != null)
            {
                WriteProperty(jsonWriter, "instance", ProblemInstance.OriginalString);
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

        /// <summary>
        /// Create problem document instance from stream of json text formatted as per media type specification
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <returns></returns>
        public static ProblemDocument Parse(Stream jsonStream)
        {
            var sr = new StreamReader(jsonStream);
            return Parse(sr.ReadToEnd());
        }

        /// <summary>
        /// Create problem document instance from string of json text
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static ProblemDocument Parse(string jsonString)
        {
            var jDoc = JObject.Parse(jsonString);
            return Parse(jDoc);
        }

        /// <summary>
        ///  Create problem document instance from JObject
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
        public static ProblemDocument Parse(JObject jObject)
        {
            var doc = new ProblemDocument();

            foreach (var jProp in jObject.Properties()){

                switch (jProp.Name)
                {
                    case "type":
                        doc.ProblemType = new Uri((string)jProp.Value, UriKind.RelativeOrAbsolute);
                        break;
                    case "title":
                        doc.Title = (string)jProp.Value;
                        break;
                    case "status":
                        doc.StatusCode = (HttpStatusCode)(int)jProp.Value;
                        break;
                    case "detail":
                        doc.Detail = (string)jProp.Value;
                        break;
                    case "instance":
                        doc.ProblemInstance = new Uri((string) jProp.Value,UriKind.RelativeOrAbsolute);
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

        /// <summary>
        /// Do a deep compare of two problem documents
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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