
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public DateOnly? DateOfBirthday { get; set; } = new DateOnly(2000, 1, 1);
        public string ImageUrl { get; set; }
        public DateOnly CreateAt { get; set; }
        public string? Address { get; set; }
        public ICollection<UserGroup> UserGroups { get; set; }
        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Attachment> SentFiles { get; set; }

    }
}
