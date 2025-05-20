using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class SDefinition
    {
        [Key]
        public Guid IdSDefinition { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NumberS { get; set; }
        public string NameEnglish { get; set; }
        public string NameJaponaise { get; set; }
        public ICollection<CritereDefinition> Critaires { get; set; } = new List<CritereDefinition>();
    }
}
