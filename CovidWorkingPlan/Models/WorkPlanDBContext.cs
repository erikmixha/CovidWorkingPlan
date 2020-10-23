using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CovidWorkingPlan.Models
{
    public partial class WorkPlanDBContext : DbContext
    {
        public WorkPlanDBContext()
        {
        }

        public WorkPlanDBContext(DbContextOptions<WorkPlanDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CompanyInfoSize> CompanyInfoSize { get; set; }
        public virtual DbSet<EmployeeInfo> EmployeeInfo { get; set; }
        public virtual DbSet<WorkingToday> WorkingToday { get; set; }

      

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CompanyInfoSize>(entity =>
            {
                entity.HasKey(e => e.IdcompanyInfo);

                entity.Property(e => e.IdcompanyInfo).HasColumnName("IDCompanyInfo");
            });

            modelBuilder.Entity<EmployeeInfo>(entity =>
            {
                entity.HasKey(e => e.Idemployee);

                entity.Property(e => e.Idemployee).HasColumnName("IDEmployee");

                entity.Property(e => e.EmpNameSurname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WorkingToday>(entity =>
            {
                entity.HasKey(e => e.IdworkToday);

                entity.Property(e => e.IdworkToday).HasColumnName("IDWorkToday");

                entity.Property(e => e.Idemployee).HasColumnName("IDEmployee");

                entity.Property(e => e.WorkDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdemployeeNavigation)
                    .WithMany(p => p.WorkingToday)
                    .HasForeignKey(d => d.Idemployee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkingToday_EmployeeInfo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
