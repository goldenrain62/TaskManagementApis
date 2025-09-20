using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class LeaderConfiguration : IEntityTypeConfiguration<Leader> {
  public void Configure(EntityTypeBuilder<Leader> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Leader> entity) {
    entity.HasKey(l => new { l.EmployeeId, l.TeamId });
    entity.Property(l => l.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(l => l.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<Leader> entity) {
    // Leaders <-> Employees
    entity.HasOne(l => l.Employee)
      .WithMany()
      .HasForeignKey(l => l.EmployeeId)
      .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(l => l.CreatedByUser) // A Leader is created by which user.
      .WithMany()
      .HasForeignKey(l => l.CreatedById)
      .OnDelete(DeleteBehavior.Restrict);

    // Leaders <-> Teams
    entity.HasOne(l => l.Team)
      .WithMany(t => t.Leaders)
      .HasForeignKey(t => t.TeamId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  public void AddSampleData(EntityTypeBuilder<Leader> entity) {
    entity.HasData(
      // Frontend Team
      new Leader { EmployeeId = 6, TeamId = 2, Leadership = "Leader" },
      new Leader { EmployeeId = 7, TeamId = 2, Leadership = "Assistant" },

      // Backend Team
      new Leader { EmployeeId = 10, TeamId = 3, Leadership = "Leader" },
      new Leader { EmployeeId = 11, TeamId = 3, Leadership = "Assistant" },

      // QA Team (TM003)
      new Leader { EmployeeId = 14, TeamId = 4, Leadership = "Leader" },
      new Leader { EmployeeId = 15, TeamId = 4, Leadership = "Assistant" },

      // DevOps Team 
      new Leader { EmployeeId = 18, TeamId = 5, Leadership = "Leader" },
      new Leader { EmployeeId = 19, TeamId = 5, Leadership = "Assistant" },

      // UI/UX Design Team
      new Leader { EmployeeId = 22, TeamId = 6, Leadership = "Leader" },
      new Leader { EmployeeId = 23, TeamId = 6, Leadership = "Assistant" },

      // HR Team
      new Leader { EmployeeId = 26, TeamId = 7, Leadership = "Leader" }
    );
  }
}
 
