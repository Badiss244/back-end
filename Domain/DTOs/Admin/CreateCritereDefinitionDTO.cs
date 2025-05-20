using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class CreateCritereDefinitionDTO
    {
        [Required] public string Name { get; set; }
        public Guid SDefinitionId { get; set; }
    }
}
