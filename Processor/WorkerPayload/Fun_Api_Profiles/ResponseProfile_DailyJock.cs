using System.Text.Json.Serialization;

namespace ScaleThreadProcess
{
    public class DailyJockResponse
    {
        [JsonPropertyName("type")]
        public string JokeType { get; set; }

        [JsonPropertyName("setup")]
        public string Setup { get; set; }

        [JsonPropertyName("punchline")]
        public string Punchline { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        public override string ToString()
        {
            return $"<strong>Joke Category</strong><br>{JokeType.ToUpper()}<hr><br><strong>The Joke</strong><br><span style=\"color: #ff6600; font-family: 'Comic Sans MS', cursive; font-size: 1.2em; background-color: #fff9c4; padding: 10px; border-radius: 8px; box-shadow: 3px 3px 5px rgba(0, 0, 0, 0.2); animation: bounce 1s infinite; display: inline-block;\">{Setup}</span><hr><br>{Punchline}";
        }
    }
}
