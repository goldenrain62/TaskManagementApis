using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class TeamConfiguration : IEntityTypeConfiguration<Team> {
  public void Configure(EntityTypeBuilder<Team> entity) {
    ConfigureProperties(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Team> entity) {
    entity.Property(t => t.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(t => t.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void AddSampleData(EntityTypeBuilder<Team> entity) {
    entity.HasData(
      new Team {
        Id = -1,
        Name = "Abstract Team",
        Description = "This team id is used only for tasks assigned for more than a team."
      },
      new Team {
        Id = 1,
        Name = "Project Manager Team",
        Description = "Responsible for the entire process of developing products."
      },
      new Team {
        Id = 2,
        Name = "Frontend Team",
        Description = "Responsible for backend and frontend development"
      },
      new Team {
        Id = 3,
        Name = "Backend Team",
        Description = "Responsible for backend and frontend development"
      },
      new Team {
        Id = 4,
        Name = "QA Team",
        Description = "Handles testing and quality assurance"
      },
      new Team {
        Id = 5,
        Name = "DevOps Team",
        Description = "Manages CI/CD pipelines and deployment"
      },
      new Team {
        Id = 6,
        Name = "UI/UX Design Team",
        Description = "Creates user interfaces and experience designs"
      },
      new Team {
        Id = 7,
        Name = "HR Team",
        Description = "Manange human resources"
      }
    );
  }
}



