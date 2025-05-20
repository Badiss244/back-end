using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public Guid IdFactory;

        public string First_name { get; set; } = "empty";
        public string Last_name { get; set; } = "empty";
        public ICollection<Nofication> Nofications { get; set; } = new List<Nofication>();

        [InverseProperty("QualityM")]
        public ICollection<PlanAction> QualityPlanActions { get; set; } = new List<PlanAction>();

        //[InverseProperty("FactoryM")]
        //public ICollection<PlanAction> FactoryPlanActions { get; set; } = new List<PlanAction>();

        [ForeignKey("Factory")]
        public Guid? FKfactory { get; set; }
        public Factory? Factory { get; set; }  

        public ICollection<Audit> Audits { get; set; } = new List<Audit>();

        public ICollection<Rapport> Rapports { get; set; } = new List<Rapport>();

        public DateTime Created { get; set; } = DateTime.Now;
        public byte[] Picture { get; set; } = ImageHelper.ConvertResourceImageToByteArray(Image.FromFile("C:\\Users\\Badis\\source\\repos\\5sNetApi\\Data\\image\\user.jpg"));

    }
}
