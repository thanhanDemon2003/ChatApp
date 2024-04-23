using Chat.Domain.Entities;
using System.Net.Mail;

namespace Chat.Domain.Entities
{
    public class Message
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public string UserRefId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public List<Attachment>? Attachments { get; set; }
    }
}