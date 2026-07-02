using Chat.App.Application.Chat.Models;
using Mapster;
using DbMessage = Chat.App.Database.Entities.Message;

namespace Chat.App.Application.Chat.Extensions;

internal static class MessageExtensions
{
    public static Message ToMessage(this DbMessage entity)
    {
        var message = entity.Adapt<Message>();
        message.SenderUsername = entity.Sender.Username;
        return message;
    }
}
