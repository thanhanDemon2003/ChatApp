using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public bool isDeleted { get; set; } = false;
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<Attachment> Files { get; set; }
    }
}
