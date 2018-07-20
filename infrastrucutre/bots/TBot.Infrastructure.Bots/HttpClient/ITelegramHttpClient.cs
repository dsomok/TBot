using System;
using System.Threading.Tasks;

namespace TBot.Infrastructure.Bots.HttpClient
{
    internal interface ITelegramHttpClient
    {
        Task<dynamic> Post(Uri uri, object body = null);
    }
}