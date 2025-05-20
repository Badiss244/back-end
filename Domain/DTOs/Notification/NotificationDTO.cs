using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Notification
{
    public class NotificationDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "Info";
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = "Empty";
        public string Username { get; set; }
        public bool Read {  get; set; }
    }
}
