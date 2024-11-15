using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ScaleThreadProcess
{
    public class MetadataFetcher
    {
        public async Task<Metadata> FetchMetadataAsync(string uri)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(uri);
                return JsonSerializer.Deserialize<Metadata>(response);
            }
        }
    }

    public class Metadata
    {
        [JsonPropertyName("datasetGroup")]
        public string DatasetGroup { get; set; }

        [JsonPropertyName("datasetName")]
        public string DatasetName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("fields")]
        public List<Field> Fields { get; set; }
    }

    public class Field
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class ClassFileWriter
    {
        public void SaveClassToFile(string classDefinition, string filePath)
        {
            File.WriteAllText(filePath, classDefinition);
        }
    }

    public class ClassGenerator
    {
        public string GenerateClass(Metadata metadata)
        {
            var codeBuilder = new StringBuilder();

            // Add namespaces and class declaration
            codeBuilder.AppendLine("using System;");
            codeBuilder.AppendLine();
            codeBuilder.AppendLine($"public class {metadata.DatasetName}Data");
            codeBuilder.AppendLine("{");

            // Generate properties based on metadata fields
            foreach (var field in metadata.Fields)
            {
                string csharpType = MapJsonTypeToCSharp(field.Type);
                codeBuilder.AppendLine($"    public {csharpType} {field.Name} {{ get; set; }}  // {field.Description}");
            }

            codeBuilder.AppendLine("}");
            return codeBuilder.ToString();
        }

        private string MapJsonTypeToCSharp(string jsonType)
        {
            string csharpType;
            switch (jsonType)
            {
                case "Number":
                    csharpType = "int";
                    break;
                case "String":
                    csharpType = "string";
                    break;
                case "Boolean":
                    csharpType = "bool";
                    break;
                case "Date":
                    csharpType = "DateTime";
                    break;
                default:
                    csharpType = "string";
                    break;
            }
            return csharpType;
        }
    }
}
