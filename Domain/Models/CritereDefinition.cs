using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class CritereDefinition
    {
        [Key]
        public Guid IdCritereDefinition { get; set; }
        public string Name { get; set; }
        [ForeignKey("SDefinition")]
        public Guid? FKsxDefinition { get; set; }
        public SDefinition? SDefinition { get; set; }
    }
}
