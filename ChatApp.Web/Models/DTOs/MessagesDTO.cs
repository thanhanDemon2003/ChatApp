using System.ComponentModel.DataAnnotations;

namespace ChatApp.Web.Models.DTOs
{
    public class MessagesDTO
    {
        public string senderId { get; set; }
        public int ConversationId { get; set; }
        public string? Content { get; set; }
        public string? AttachmentUrl { get; set; }
        public string? AttachmentType { get; set; }

    }
}
