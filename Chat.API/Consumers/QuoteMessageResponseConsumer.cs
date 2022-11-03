using Chat.API.Database;
using Chat.API.Hubs;
using Chat.Shared.BrokerContracts;
using Chat.Shared.Helpers;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Consumers
{
    public class QuoteMessageResponseConsumer : IConsumer<IQuoteMessageResponse>
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatContext _dbContext;

        public QuoteMessageResponseConsumer(IHubContext<ChatHub> hubContext,
            ChatContext dbContext)
        {
            _hubContext = hubContext;
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<IQuoteMessageResponse> context)
        {
            var currentDate = DateTime.Now;
            if (context.Message.Success)
            {
                var dbRecord = new ChatRoomMessage
                {
                    ChatRoomId = context.Message.ChatRoomId,
                    UserName = "Quote Bot",
                    Created = currentDate,
                    Message = context.Message.Quote
                };

                _dbContext.ChatRoomMessage.Add(dbRecord);
                await _dbContext.SaveChangesAsync();
            }

            var message = ChatMessageBuilder.FormatMessage(
                context.Message.Success ? context.Message.Quote : context.Message.Error, currentDate, true);
            await _hubContext.Clients.Groups(context.Message.ChatRoomId.ToString())
                .SendAsync("ReceiveMessage", message);
        }
    }
}

