using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace FinraEDSProcessor.Classes
{
    public class ConfigLoader
    {
        public static async Task<ProfileConfig> LoadConfigAsync(string filePath)
        {
            FileStream fs = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<ProfileConfig>(fs);
        }
    }
}