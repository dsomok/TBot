namespace TBot.Infrastructure
{
    public interface ISerializer
    {
        string Serialize<T>(T data);
        byte[] SerializeAsBytes<T>(T data);

        T Deserialize<T>(string content);
        T Deserialize<T>(byte[] content);
    }
}