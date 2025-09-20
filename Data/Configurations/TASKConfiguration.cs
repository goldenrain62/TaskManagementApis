using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class TASKConfiguration : IEntityTypeConfiguration<TASK> {
  public void Configure(EntityTypeBuilder<TASK> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<TASK> entity) {
    entity.Property(t => t.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(t => t.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<TASK> entity) {
    // Tasks <-> Categories 
    entity.HasOne(t => t.Category)
      .WithMany(c => c.Tasks)
      .HasForeignKey(t => t.CategoryId)
      .OnDelete(DeleteBehavior.Restrict);

    // Tasks <-> Employees 
    entity.HasOne(t => t.CreatedByUser) // A Task is created by which user(employee)
      .WithMany(e => e.CreatedTasks)
      .HasForeignKey(t => t.CreatedById)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(t => t.UpdatedByUser) // A Task is updated by which user(employee). 
      .WithMany()
      .HasForeignKey(t => t.UpdatedById)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(t => t.ExecutedByEmployee)
      .WithMany(e => e.AssignedTasks)
      .HasForeignKey(t => t.ExecutedByEmployeeId)
      .OnDelete(DeleteBehavior.Restrict);

    // Tasks <-> Teams
    entity.HasOne(t => t.ExecutedByTeam)
      .WithMany(tm => tm.AssignedTasks)
      .HasForeignKey(t => t.ExecutedByTeamId)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(t => t.CreatedByTeam)
      .WithMany(tm => tm.CreatedTasks)
      .HasForeignKey(t => t.CreatedByTeamId)
      .OnDelete(DeleteBehavior.Restrict);

    // Tasks <-> Tasks
    entity.HasOne(t => t.ParentTask)
      .WithMany(pt => pt.SubTasks)
      .HasForeignKey(t => t.ParentTaskId)
      .OnDelete(DeleteBehavior.Restrict); 

    // Tasks <-> TaskStates
    entity.HasOne(t => t.CurrentState)
      .WithMany(ts => ts.Tasks)
      .HasForeignKey(t => t.CurrentStateId)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
