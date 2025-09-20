using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category> {
  public void Configure(EntityTypeBuilder<Category> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Category> entity) {
    entity.Property(c => c.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(c => c.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<Category> entity) {
    // Categories <-> Employees
    entity.HasOne(c => c.CreatedByUser) // A Category is created by which user(employee).
      .WithMany()
      .HasForeignKey(c => c.CreatedById)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(c => c.UpdatedByUser) // A Category is updated by which user(employee).
      .WithMany()
      .HasForeignKey(c => c.UpdatedById)
      .OnDelete(DeleteBehavior.Restrict);

    // Categories <-> Teams
    entity.HasOne(c => c.CreatedByTeam) // A Category is created by which team.
      .WithMany()
      .HasForeignKey(c => c.CreatedByTeamId)
      .OnDelete(DeleteBehavior.Restrict);
  }

  public void AddSampleData(EntityTypeBuilder<Category> entity) {
    entity.HasData(
      new Category { Id = 1, Name = "Initializing a new project" },
      new Category { Id = 2, Name = "Planning" },
      new Category { Id = 3, Name = "Design" },
      new Category { Id = 4, Name = "Development" },
      new Category { Id = 5, Name = "Testing" },
      new Category { Id = 6, Name = "Deployment" },
      new Category { Id = 7, Name = "Maintenance" }
    );
  }
}
 
