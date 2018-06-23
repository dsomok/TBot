using System;
using Newtonsoft.Json;

namespace TBot.Infrastructure
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
