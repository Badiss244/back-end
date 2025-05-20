using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Tache
    {
        [Key]
        public Guid IdTache { get; set; }
        public string Name { get; set; }
        public string? NameS { get; set; }
        public bool IsDone { get; set; } = false;
        [ForeignKey("PlanAction")]
        public Guid FKplanaction { get; set; }
        public List<byte[]> Pictures { get; set; }= new List<byte[]>();
        public string Commantaire { get; set; } = "Empty";
        public PlanAction PlanAction { get; set; }
    }
}
