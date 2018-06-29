using System;
using System.Text;
using Newtonsoft.Json;

namespace TBot.Infrastructure
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize<T>(T data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public byte[] SerializeAsBytes<T>(T data)
        {
            var json = this.Serialize(data);
            return Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }

        public T Deserialize<T>(byte[] content)
        {
            var json = Encoding.UTF8.GetString(content);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
