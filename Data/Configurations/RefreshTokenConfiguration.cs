using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Services;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken> {
  public void Configure(EntityTypeBuilder<RefreshToken> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<RefreshToken> entity) {
    entity.HasIndex(rt => new { rt.UserId, rt.TokenHash }).IsUnique();
    entity.Property(rt => rt.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(rt => rt.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<RefreshToken> entity) { 
    entity.HasOne(rt => rt.Account)
      .WithMany(a => a.RefreshTokens)
      .HasForeignKey(rt => rt.UserId)
      .OnDelete(DeleteBehavior.Cascade);
  }
}
 
