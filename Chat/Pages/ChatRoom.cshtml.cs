using Chat.Client;
using Chat.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chat.Pages
{
    public class ChatRoomModel : PageModel
    {
        private readonly ChatHttpClient _client;

        public ChatRoomModel(ChatHttpClient client)
        {
            _client = client;
        }

        public async Task OnGet()
        {
            ChatRoomId = 1;
            ChatRoomMessages = await _client.GetChatRoomMessages(ChatRoomId);
            
        }

        public int ChatRoomId { get; set; }

        public GetChatRoomMessageItemResponseDto[] ChatRoomMessages { get; set; }

        public async Task<IActionResult> OnPostAddMessage([FromBody] SendMessageToChatDto message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _client.SendChatRoomMessage(message);
            return new OkResult();
        }

        public async Task<IActionResult> OnGetToken()
        {
            var token = await HttpContext.GetUserAccessTokenAsync();
            return new OkObjectResult(new {accessToken = token.AccessToken});
        }
    }
}
