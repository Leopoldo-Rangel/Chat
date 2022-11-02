using Chat.Dto;

namespace Chat.Client
{
    public class ChatHttpClient
    {
        private readonly HttpClient _httpClient;

        public ChatHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task SendChatRoomMessage(SendMessageToChatDto message)
        {
            var response = await _httpClient.PostAsJsonAsync($"/ChatRooms/{message.ChatGroupId}/SendMessage", message);
            response.EnsureSuccessStatusCode();
        }

        public async Task<GetChatRoomMessageItemResponseDto[]> GetChatRoomMessages(int chatRoomId)
        {
            var response = await _httpClient.GetFromJsonAsync<GetChatRoomMessageItemResponseDto[]>($"/ChatRooms/{chatRoomId}/GetMessages");
            return response!;
        }

        public async Task<GetChatRoomItemResponseDto[]> GetChatRooms()
        {
            var response = await _httpClient.GetFromJsonAsync<GetChatRoomItemResponseDto[]>($"/ChatRooms");
            return response!;
        }

        public async Task<GetChatRoomItemResponseDto> GetChatRoom(int chatRoomId)
        {
            var response = await _httpClient.GetFromJsonAsync<GetChatRoomItemResponseDto>($"/ChatRooms/{chatRoomId}");
            return response!;
        }
    }
}
