using System.Text.Json;

namespace ScaleThreadProcess
{
    public class ConfigLoader
    {
        public static ApiConfig LoadApiConfig(string jsonContent)
        {
            return JsonSerializer.Deserialize<ApiConfig>(jsonContent);
        }
    }
}