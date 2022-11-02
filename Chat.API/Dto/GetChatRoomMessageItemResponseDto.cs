namespace Chat.API.Dto
{
    public class GetChatRoomMessageItemResponseDto
    {
        public int? Id { get; set; }
        public string? UserName { get; set; }
        public string? Message { get; set; }
        public DateTime? Created { get; set; }
    }
}
