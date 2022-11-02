using System.ComponentModel.DataAnnotations;

namespace Chat.Dto
{
    public class SendMessageToChatDto
    {
        [Required]
        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;
        [Required]
        public int ChatGroupId { get; set; }
    }
}
