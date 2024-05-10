using Chat.Domain.Entities;

namespace ChatApp.Web.ViewModels
{
    public class MessageVM
    {
        public List<Message> messages { get; set; }
        public string conversationId { get; set; }
        public string conversationName { get; set; }
        public string sendToId { get; set; }
        public bool isGroup { get; set; }
    }
}
