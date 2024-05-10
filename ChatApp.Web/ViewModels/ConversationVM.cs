using Chat.Domain.Entities;
using ChatApp.Web.Models.DTOs;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ChatApp.Web.ViewModels
{
    public class ConversationVM
    {
        public IEnumerable<Conversation>? conversationList { get; set; }
        [ValidateNever]
        public IEnumerable<UserDTO>? userList { get; set; }


    }
}
