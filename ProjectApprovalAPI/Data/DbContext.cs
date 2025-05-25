using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectApprovalAPI.Models;
using System.IO;

namespace ProjectApprovalAPI.Data
{
    public class ProjectApprovalDbContext : DbContext
    {
        private readonly string _connectionString;
        public ProjectApprovalDbContext(DbContextOptions<ProjectApprovalDbContext> options) : base(options)
        {
        }

        public DbSet<Area> Areas { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatuses { get; set; }
        public DbSet<ApproverRole> ApproverRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ApprovalRule> ApprovalRules { get; set; }
        public DbSet<ProjectProposal> ProjectProposals { get; set; }
        public DbSet<ProjectApprovalStep> ProjectApprovalSteps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectApprovalStep>()
                .HasOne(p => p.ProjectProposal)
                .WithMany()
                .HasForeignKey(p => p.ProjectProposalId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectApprovalStep>()
                .HasOne(s => s.ProjectProposal)
                .WithMany(p => p.ApprovalSteps)
                .HasForeignKey(s => s.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectApprovalStep>()
                .HasOne(s => s.ApproverUser)
                .WithMany(u => u.ApproverSteps)
                .HasForeignKey(s => s.ApproverUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectApprovalStep>()
                .HasOne(s => s.ApproverRole)
                .WithMany(ar => ar.ProjectApprovalSteps)
                .HasForeignKey(s => s.ApproverRoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectApprovalStep>()
                .HasOne(s => s.Status)
                .WithMany(status => status.ProjectApprovalSteps)
                .HasForeignKey(s => s.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalStatus>().HasData(
                new ApprovalStatus { Id = 1, Name = "Pending" },
                new ApprovalStatus { Id = 2, Name = "Approved" },
                new ApprovalStatus { Id = 3, Name = "Rejected" },
                new ApprovalStatus { Id = 4, Name = "Observed" }
            );

            modelBuilder.Entity<ApproverRole>().HasData(
                new ApproverRole { Id = 1, Name = "Líder de Área" },
                new ApproverRole { Id = 2, Name = "Gerente" },
                new ApproverRole { Id = 3, Name = "Director" },
                new ApproverRole { Id = 4, Name = "Comité Técnico" }
            );

            modelBuilder.Entity<Area>().HasData(
                new Area { Id = 1, Name = "Finanzas" },
                new Area { Id = 2, Name = "Tecnología" },
                new Area { Id = 3, Name = "Recursos Humanos" },
                new Area { Id = 4, Name = "Operaciones" }
            );

            modelBuilder.Entity<ProjectType>().HasData(
                new ProjectType { Id = 1, Name = "Mejora de Procesos" },
                new ProjectType { Id = 2, Name = "Innovación y Desarrollo" },
                new ProjectType { Id = 3, Name = "Infraestructura" },
                new ProjectType { Id = 4, Name = "Capacitación Interna" }
            );

            modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "José Ferreyra", Email = "jferreyra@unaj.com", RoleId = 2 },
            new User { Id = 2, Name = "Ana Lucero", Email = "alucero@unaj.com", RoleId = 1 },
            new User { Id = 3, Name = "Gonzalo Molinas", Email = "gmolinas@unaj.com", RoleId = 2 },
            new User { Id = 4, Name = "Lucas Olivera", Email = "lolivera@unaj.com", RoleId = 3 },
            new User { Id = 5, Name = "Danilo Fagundez", Email = "dfagundez@unaj.com", RoleId = 4 },
            new User { Id = 6, Name = "Gabriel Galli", Email = "ggalli@unaj.com", RoleId = 4 }
            );

            modelBuilder.Entity<ApprovalRule>().HasData(
                new ApprovalRule { Id = 1, MinAmount = 0, MaxAmount = 100000, AreaId = null, TypeId = null, StepOrder = 1, ApproverRoleId = 1 },
                new ApprovalRule { Id = 2, MinAmount = 5000, MaxAmount = 20000, AreaId = null, TypeId = null, StepOrder = 2, ApproverRoleId = 2 },
                new ApprovalRule { Id = 3, MinAmount = 0, MaxAmount = 20000, AreaId = 2, TypeId = 2, StepOrder = 1, ApproverRoleId = 2 },
                new ApprovalRule { Id = 4, MinAmount = 20000, MaxAmount = 0, AreaId = null, TypeId = null, StepOrder = 3, ApproverRoleId = 3 },
                new ApprovalRule { Id = 5, MinAmount = 5000, MaxAmount = 0, AreaId = 1, TypeId = 1, StepOrder = 2, ApproverRoleId = 2 },
                new ApprovalRule { Id = 6, MinAmount = 0, MaxAmount = 10000, AreaId = null, TypeId = 2, StepOrder = 1, ApproverRoleId = 1 },
                new ApprovalRule { Id = 7, MinAmount = 0, MaxAmount = 10000, AreaId = 2, TypeId = 1, StepOrder = 4, ApproverRoleId = 1 },
                new ApprovalRule { Id = 8, MinAmount = 10000, MaxAmount = 30000, AreaId = 2, TypeId = null, StepOrder = 2, ApproverRoleId = 2 },
                new ApprovalRule { Id = 9, MinAmount = 30000, MaxAmount = 0, AreaId = 3, TypeId = null, StepOrder = 2, ApproverRoleId = 3 },
                new ApprovalRule { Id = 10, MinAmount = 0, MaxAmount = 50000, AreaId = null, TypeId = 4, StepOrder = 1, ApproverRoleId = 4 }
            );
        }
    }
}
