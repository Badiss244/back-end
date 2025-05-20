using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Domain.Models
{
    public class Rapport
    {
        [Key]
        public Guid IdRapport { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsDone { get; set; } = false;
        [ForeignKey("AppUser")]
        public Guid? FKauditor {  get; set; }
        [ForeignKey("Factory")]
        public Guid? FKfactory { get; set; }
        public Factory? Factory { get; set; }
        public AppUser? AppUser { get; set; }
        public ICollection<Evidence> Evidence { get; set; } = new List<Evidence>();
        public ICollection<PlanAction> PlanActions { get; set; } = new List<PlanAction>();

        public Guid X { get; set; } = Guid.Empty;
    }
}
