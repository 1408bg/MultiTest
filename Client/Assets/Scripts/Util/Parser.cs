using Newtonsoft.Json;

namespace Util
{
    public class Parser
    {
        public static T ParseJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}