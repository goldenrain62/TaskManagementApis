using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Services;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class TaskStateConfiguration : IEntityTypeConfiguration<TaskState> {
  public void Configure(EntityTypeBuilder<TaskState> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<TaskState> entity) {
    entity.Property(ts => ts.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(ts => ts.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<TaskState> entity) {
    // TaskStates <-> Employees
    entity.HasOne(ts => ts.CreatedBy)
      .WithMany(e => e.TaskStatesCreated)
      .HasForeignKey(ts => ts.CreatedById)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(ts => ts.UpdatedBy)
      .WithMany()
      .HasForeignKey(ts => ts.UpdatedById)
      .OnDelete(DeleteBehavior.Restrict);

    // TaskStates <-> Teams
    entity.HasOne(ts => ts.CreatedByTeam)
      .WithMany()
      .HasForeignKey(ts => ts.CreatedByTeamId)
      .OnDelete(DeleteBehavior.Restrict);
  }

  public void AddSampleData(EntityTypeBuilder<TaskState> entity) {
    entity.HasData(
      new TaskState {
        Id = 1,
        Name = "Planning",
        Description = "Project is being scoped, discussed, or brainstormed.",
      },
      new TaskState {
        Id = 2,
        Name = "Planned",
        Description = "Project has a defined scope, timeline, and resources.",
      },
      new TaskState {
        Id = 3,
        Name = "Designing",
        Description = "Project is in the design phase — wireframes, architecture, or UX.",
      },
      new TaskState {
        Id = 4,
        Name = "Designed",
        Description = "Design work is completed and ready for review or handoff.",
      },
      new TaskState {
        Id = 5,
        Name = "Ready For Devs",
        Description = "Project is fully prepared and queued for development.",
      },
      new TaskState {
        Id = 6,
        Name = "In Progress",
        Description = "Actively being worked on by a developer or team.",
      },
      new TaskState {
        Id = 7,
        Name = "Blocked",
        Description = "Progress is halted due to dependencies, issues, or missing input.",
      },
      new TaskState {
        Id = 8,
        Name = "Ready for QA",
        Description = "Development is complete; task is ready for testing.",
      },
      new TaskState {
        Id = 9,
        Name = "In QA",
        Description = "Project is undergoing testing and validation by QA team.",
      },
      new TaskState {
        Id = 10,
        Name = "Ready for Release",
        Description = "QA passed; Project is approved for deployment.",
      },
      new TaskState {
        Id = 11,
        Name = "Deploying",
        Description = "Project is currently being released to staging or production.",
      },
      new TaskState {
        Id = 12,
        Name = "Deployed",
        Description = "Project has been successfully released.",
      },
      new TaskState {
        Id = 13,
        Name = "Verifying",
        Description = "Post-deployment checks are underway to confirm success.",
      },
      new TaskState {
        Id = 14,
        Name = "Verified",
        Description = "Project has passed all post-deployment checks.",
      },
      new TaskState {
        Id = 15,
        Name = "Done",
        Description = "Project is fully completed and closed.",
      },
      new TaskState {
        Id = 16,
        Name = "Archived",
        Description = "Project is no longer active but retained for historical reference.",
      },
      new TaskState {
        Id = 17,
        Name = "Canceled",
        Description = "Task was intentionally stopped and will not be completed.",
      },
      new TaskState {
        Id = 18,
        Name = "Pending Approval",
        Description = "Task awaits sign-off from a stakeholder or reviewer.",
      },
      new TaskState {
        Id = 19,
        Name = "In Review",
        Description = "Task is under active evaluation for quality or correctness.",
      }
    );
  }
}
