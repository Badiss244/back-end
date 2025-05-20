using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Notification
{
    public class CreateNotificationDTO
    {
        public string? RoleDes { get; set; }
        public List<string>? Usernames { get; set; }
        public string Type { get; set; } = "Info";
        public string Message { get; set; } = "msgDescriptionHere";
    }
}
