using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role> {
  public void Configure(EntityTypeBuilder<Role> entity) {
    ConfigureProperties(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Role> entity) {
    entity.HasIndex(r => r.RoleName)
      .IsUnique();
    entity.Property(r => r.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(r => r.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void AddSampleData(EntityTypeBuilder<Role> entity) {
    entity.HasData(
      new Role { Id = 1, RoleName = "System Admin", Description = "Describe System Admin role." },
      new Role { Id = 2, RoleName = "Project Manager", Description = "Describe Project Manager role." },
      new Role { Id = 3, RoleName = "Frontend Leader", Description = "Describe Frontend Leader role." },
      new Role { Id = 4, RoleName = "Backend Leader", Description = "Describe Backend Leader role." },
      new Role { Id = 5, RoleName = "QA Leader", Description = "Describe QA Leader role." },
      new Role { Id = 6, RoleName = "DevOps Leader", Description = "Describe DevOps Leader role." },
      new Role { Id = 7, RoleName = "UI/UX Design Leader", Description = "Describe UI/UX Design Leader role." },
      new Role { Id = 8, RoleName = "HR", Description = "Describe HR Leader role." },
      new Role { Id = 9, RoleName = "Employee", Description = "Describe Employee Leader role." }
    );
  }
}
