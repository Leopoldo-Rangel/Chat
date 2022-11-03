using Chat.API.Database;
using Chat.API.Dto;
using Chat.API.Hubs;
using Chat.Shared.BrokerContracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Chat.Shared.Helpers;

namespace Chat.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomsController  : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public ChatRoomsController(IHubContext<ChatHub> hubContext, 
            ChatContext context,
            ISendEndpointProvider sendEndpointProvider)
        {
            _hubContext = hubContext;
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetChatRooms()
        {
            var chatRooms = await _context.ChatRoom
                .Select(c => new ChatRoom
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name
                })
                .ToArrayAsync();

            return Ok(chatRooms);
        }

        [HttpGet("{chatRoomId:int}")]
        public async Task<IActionResult> GetChatRoom(int chatRoomId)
        {
            var chatRoom = await _context.ChatRoom
                .Where(c => c.Id == chatRoomId)
                .Select(c => new ChatRoom
                {
                    Id = c.Id,
                    Description = c.Description,
                    Name = c.Name
                })
                .SingleOrDefaultAsync();

            if (chatRoom == null) return NotFound();
            return Ok(chatRoom);
        }

        [HttpGet("{chatRoomId:int}/[action]")]
        public async Task<IActionResult> GetMessages(int chatRoomId)
        {
            var chatRoomMessages = await _context.ChatRoomMessage
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderByDescending(d => d.Created)
                .Take(50)
                .Select(m => new GetChatRoomMessageItemResponseDto
                {
                    Id = m.Id,
                    Message = m.Message, 
                    UserName = m.UserName,
                    Created = m.Created
                })
                .ToArrayAsync();

            return Ok(chatRoomMessages.OrderBy(d => d.Created));
        }

        [HttpPost("{chatRoomId:int}/[action]")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageToChatRequestDto dto,
            [FromRoute] int chatRoomId)
        {
            if (dto.Message.ToLower().StartsWith("/stock"))
            {
                var stockRequestParts = dto.Message.Split("=", StringSplitOptions.RemoveEmptyEntries);
                if (stockRequestParts.Length != 2)
                {
                    var error = "Bad request, stock instruction is missing stock name";
                    var errorMessage = ChatMessageBuilder.FormatMessage(error, DateTime.Now, true);
                    await _hubContext.Clients.Groups(chatRoomId.ToString()).SendAsync("ReceiveMessage", errorMessage);
                    return BadRequest(new { Error = error });
                }

                var stockName = stockRequestParts[1].Trim();
                var successMessage = ChatMessageBuilder.FormatMessage(
                    $"Petition to quote stock {stockName} received, processing...", DateTime.Now, true);
                await _hubContext.Clients.Groups(chatRoomId.ToString()).SendAsync("ReceiveMessage", successMessage);
                await _sendEndpointProvider.Send<IAddQuoteMessage>(new
                {
                    StockName = stockName,
                    ChatRoomId = chatRoomId
                });

                return Accepted();
            }

            var userName = User.FindFirstValue(ClaimTypes.Name);
            var record = new ChatRoomMessage
            {
                ChatRoomId = chatRoomId, Created = DateTime.Now, Message = dto.Message, UserName = userName
            };
            _context.ChatRoomMessage.Add(record);
            await _context.SaveChangesAsync();

            var message = ChatMessageBuilder.FormatMessage(dto.Message, record.Created, false, record.UserName);
            await _hubContext.Clients.Groups(chatRoomId.ToString()).SendAsync("ReceiveMessage", message);
            return Ok();
        }
    }
}
