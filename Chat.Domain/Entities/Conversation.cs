using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public string Name { get; set; }
        public ConversationType ConversationType { get; set; }
        public List<Message>? Messages { get; set; }
        public List<Attachment>? Attachments { get; set; }
        public List<UserConversation>? UserConversations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public enum ConversationType
    {
        Private,
        Group
    }
}
