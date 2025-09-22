using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
  public void Configure(EntityTypeBuilder<Comment> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Comment> entity) {
    entity.Property(c => c.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(c => c.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<Comment> entity) {
    // Comments <-> Tasks
    entity.HasOne(c => c.Task)
      .WithMany(t => t.Comments)
      .HasForeignKey(c => c.TaskId)
      .OnDelete(DeleteBehavior.Cascade);

    // Comments <-> Accounts
    entity.HasOne(c => c.CommentedBy)
      .WithMany(a => a.Comments)
      .HasForeignKey(c => c.CommentedById)
      .OnDelete(DeleteBehavior.Cascade);

    // Comments <-> Comments
    entity.HasOne(c => c.ParentComment)
      .WithMany(pc => pc.Replies)
      .HasForeignKey(c => c.RepliesTo)
      .OnDelete(DeleteBehavior.Restrict);
  }
}
