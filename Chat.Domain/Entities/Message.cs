using System.ComponentModel.DataAnnotations;

namespace Chat.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public int? GroupId { get; set; }
        public string? ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentDate { get; set; }
        public bool isDeleted { get; set; } = false;
        public ApplicationUser User { get; set; }
        public Group Group { get; set; }
        public ApplicationUser Receiver { get; set; }
    }
}