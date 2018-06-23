using System;

namespace TBot.Infrastructure.Bots.HttpClient
{
    internal interface ITelegramHttpClient
    {
        System.Threading.Tasks.Task<dynamic> Post(Uri uri, object body = null);
    }
}