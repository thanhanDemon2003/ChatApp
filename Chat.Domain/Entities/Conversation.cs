
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Chat.Domain.Entities
{
    public class Conversation
    {
        [Key]
        public int ConversationId { get; set; }
        public string UserRefId { get; set; }
        public int? GroupId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime LatestMessageDateTime { get; set; }
    }
}