using Chat.Domain.Entities;

namespace Chat.Domain.Entities
{
    public class UserConversation
    {
        public string UserRefId { get; set; }
        public ApplicationUser User { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        public Guid Id { get; set; }
    }
}