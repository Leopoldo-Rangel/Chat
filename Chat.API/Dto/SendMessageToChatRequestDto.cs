using System.ComponentModel.DataAnnotations;

namespace Chat.API.Dto
{
    public class SendMessageToChatRequestDto
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
    }
}
