using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin
{
    public class UpdateCritereDefinitionDTO
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? SxId { get; set; }
    }
}
