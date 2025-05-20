using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Nofication
    {
        [Key]
        public Guid IdNofication { get; set; }
        public String? RoleDes { get; set; }
        public Guid? IdDes { get; set; }
        public string Type { get; set; } = "Info";
        public bool Read { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Message { get; set; } = "msgDescriptionHere";
        [ForeignKey("AppUser")]
        public Guid FKappuser { get; set; }
        public AppUser AppUser { get; set; }
    }
}
