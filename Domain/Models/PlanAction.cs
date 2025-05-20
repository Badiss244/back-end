using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models
{
    public class PlanAction
    {
        [Key]
        public Guid IdPlanAction { get; set; }

        public string Name { get; set; } = "plan";

        public bool IsDone { get; set; } = false;
        [ForeignKey("Factory")]
        public Guid FKfactory {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid? FKqualitym { get; set; }
        //public Guid FKfactorym { get; set; }

        [ForeignKey(nameof(FKqualitym))]
        [InverseProperty("QualityPlanActions")]
        public AppUser? QualityM { get; set; }   //tnajim tzid ? 7asb 7alla

        //[ForeignKey(nameof(FKfactorym))]
        //[InverseProperty("FactoryPlanActions")]
        //public AppUser FactoryM { get; set; }  //tnajim tzid ? 7asb 7alla

        [ForeignKey("Rapport")]
        public Guid? FKrapport { get; set; }
        public Rapport? Rapport { get; set; }
        public ICollection<Tache> Taches { get; set; } = new List<Tache>();
        public Factory Factory { get; set; }
    }
}
