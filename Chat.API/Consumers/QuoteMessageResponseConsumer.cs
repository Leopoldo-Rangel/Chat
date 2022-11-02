using Chat.API.Database;
using Chat.API.Hubs;
using Chat.Shared.BrokerContracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Consumers
{
    public class QuoteMessageResponseConsumer : IConsumer<IQuoteMessageResponse>
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public QuoteMessageResponseConsumer(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IQuoteMessageResponse> context)
        {
            if (context.Message.Success)
            {
                await using var dbContext = new ChatContext();
                dbContext.ChatRoomMessage.Add(new ChatRoomMessage
                {
                    ChatRoomId = context.Message.ChatRoomId,
                    UserName = "Quote Bot",
                    Created = DateTime.Now,
                    Message = context.Message.Quote
                });

                await dbContext.SaveChangesAsync();
            }

            await _hubContext.Clients.Groups(context.Message.ChatRoomId.ToString())
                .SendAsync("ReceiveMessage", context.Message.Success 
                    ? $"Quote Bot says: {context.Message.Quote}"
                    : $"Quote Bot says: {context.Message.Error}");
        }
    }
}
