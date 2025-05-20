using System.Reflection.Metadata;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<Critaire> Critaires { get; set; }
        public DbSet<Evidence> Evidence { get; set; }
        public DbSet<Factory> Factories { get; set; }
        public DbSet<Filiale> Filiales { get; set; }
        public DbSet<Nofication> Nofications { get; set; }
        public DbSet<PlanAction> PlanActions { get; set; }
        public DbSet<Rapport> Rapports { get; set; }
        public DbSet<Sx> Sx { get; set; }
        public DbSet<Tache> Taches { get; set; }
        public DbSet<SDefinition> SxDefinitions { get; set; }
        public DbSet<CritereDefinition> critereDefinitions { get; set; }
        public DbSet<Parametres> Parametres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Parametres>().HasData(new Parametres
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001")
            });


            modelBuilder.Entity<PlanAction>()
                .HasOne(pa => pa.QualityM)
                .WithMany(u => u.QualityPlanActions)
                .HasForeignKey(pa => pa.FKqualitym)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanAction>()
                .HasOne(pa => pa.Factory)
                .WithMany(f => f.PlanActions)
                .HasForeignKey(pa => pa.FKfactory)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Rapport>()
                .HasOne(r => r.Factory)
                .WithMany(f => f.Rapports)
                .HasForeignKey(r => r.FKfactory)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Audit>()
                .HasOne(a => a.Factory)
                .WithMany(f => f.Audits)
                .HasForeignKey(a => a.FKfactory)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evidence>()
                .HasOne(e => e.Rapport)
                .WithMany(r => r.Evidence)
                .HasForeignKey(e => e.FKrapport)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tache>()
                .HasOne(t => t.PlanAction)
                .WithMany(pa => pa.Taches)
                .HasForeignKey(t => t.FKplanaction)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Factory>()
                .HasOne(f => f.Filiale)
                .WithMany(fi => fi.Factories)
                .HasForeignKey(f => f.FKfiliale)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Nofication>()
                .HasOne(n => n.AppUser)
                .WithMany(u => u.Nofications)
                .HasForeignKey(n => n.FKappuser)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Critaire>()
                .HasOne(c => c.Sx)
                .WithMany(s => s.Critaires)
                .HasForeignKey(c => c.FKsx)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CritereDefinition>()
                .HasOne(c => c.SDefinition)
                .WithMany(s => s.Critaires)
                .HasForeignKey(c => c.FKsxDefinition)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Sx>()
                .HasOne(s => s.Factory)
                .WithMany(f => f.Sx)
                .HasForeignKey(s => s.FKfactory)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Factory>()
                .HasOne(u => u.AppUser)
                .WithOne(f => f.Factory)
                .HasForeignKey<AppUser>(u => u.FKfactory)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
