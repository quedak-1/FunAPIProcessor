using System.Text.Json.Serialization;

namespace ScaleThreadProcess
{
    public class CatFactsResponse
    {
        [JsonPropertyName("fact")]
        public string Fact { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        public override string ToString()
        {
            return $"<strong>Cat Fact</strong><br><br><span style=\"color: #ff6600; font-family: 'Comic Sans MS', cursive; font-size: 1.2em; background-color: #fff9c4; padding: 10px; border-radius: 8px; box-shadow: 3px 3px 5px rgba(0, 0, 0, 0.2); animation: bounce 1s infinite; display: inline-block;\">{Fact}</span>";
        }
    }
}
