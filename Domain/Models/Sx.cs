using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Sx
    {
        [Key]
        public Guid IdSx { get; set; }
        public int NumberS {  get; set; }
        public string NameEnglish { get; set; }
        public string NameJaponaise { get; set; }
        [ForeignKey("Factory")]
        public Guid FKfactory { get; set; }
        public Factory Factory { get; set; }
        public ICollection<Critaire> Critaires { get; set; } = new List<Critaire>();

        public double CalculerMoyenne()
        {
            if (Critaires == null || Critaires.Count == 0)
            {
                return 0;
            }
            return Critaires.Average(c => c.Score);
        }
    }
}
