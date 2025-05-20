using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Audit
    {
        [Key]
        public Guid IdAudit { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateOnly PlanDate { get; set; }
        public bool IsOld { get; set; } = false;
        [ForeignKey("Factory")]
        public string Status { get; set; } = "Underway";
        public Guid? FKfactory {  get; set; }
        [ForeignKey("AppUser")]
        public Guid? FKauditor { get; set; }
        public Factory? Factory { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
