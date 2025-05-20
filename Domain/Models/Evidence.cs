using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Evidence
    {
        [Key]
        public Guid IdEvidence { get; set; }
        public byte[] Picture { get; set; }
        [ForeignKey("Rapport")]
        public Guid FKrapport {  get; set; }
        public Rapport Rapport { get; set; }
    }
}
