using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Domain.Models
{
    public class Factory
    {
        [Key]
        public Guid IdFactory { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [ForeignKey("Filiale")]
        public Guid FKfiliale { get; set; }

        public Filiale Filiale { get; set; }

        public ICollection<Sx> Sx { get; set; } = new List<Sx>();

        public ICollection<Audit> Audits { get; set; } = new List<Audit>();

        public ICollection<Rapport> Rapports { get; set; } = new List<Rapport>();
        public ICollection<PlanAction> PlanActions { get; set; } = new List<PlanAction>();

        // Navigation property for the related AppUser
        public AppUser? AppUser { get; set; }

        public double CalculerMoyenneGlobal()
        {
            if (Sx == null || Sx.Count == 0)
            {
                return 0;
            }

            // Calculer la moyenne pour chaque Sx, puis la moyenne globale
            double total = Sx.Sum(s => s.CalculerMoyenne());
            return total / Sx.Count;
        }
    }
}
