using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Services;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account> {
  public void Configure(EntityTypeBuilder<Account> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Account> entity) {
    entity.HasIndex(a => a.Username)
      .IsUnique();
    entity.Property(a => a.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(a => a.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<Account> entity) {
    // Accounts <-> Employees
    entity.HasOne(a => a.BelongsTo)
      .WithOne()
      .HasForeignKey<Account>(a => a.Id)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(a => a.UpdatedByUser)  // An Account is updated data by which user(employee).
      .WithMany()
      .HasForeignKey(a => a.UpdatedById)
      .OnDelete(DeleteBehavior.Restrict);

    // Accounts <-> Roles
    entity.HasOne(a => a.Role)
      .WithMany(r => r.Accounts)
      .HasForeignKey(a => a.RoleId)
      .OnDelete(DeleteBehavior.Restrict);
  }

  public void AddSampleData(EntityTypeBuilder<Account> entity) {
    HasherService hasher = new HasherService();

    entity.HasData(
      new Account {
        Id = 1,
        Username = "sa",
        Password = hasher.ComputeMD5Hash("system.admin111"),
        State = "Active",
        RoleId = 1,
      },
      // Team 2: Project Manager Team
      new Account { Id = 2, Username = "Emily.Johnson", Password = hasher.ComputeMD5Hash("Emily.Johnson123"), State = "Active", RoleId = 2 },
      new Account { Id = 3, Username = "Haruki.Saito", Password = hasher.ComputeMD5Hash("Haruki.Saito123"), State = "Active", RoleId = 2 },
      new Account { Id = 4, Username = "Huong.Tran", Password = hasher.ComputeMD5Hash("Huong.Tran123"), State = "Active", RoleId = 2 },
      new Account { Id = 5, Username = "Ginny.Lewis", Password = hasher.ComputeMD5Hash("Ginny.Lewis123"), State = "Active", RoleId = 2 },

      // Team 3: Frontend Team
      new Account { Id = 6, Username = "Minho.Lee", Password = hasher.ComputeMD5Hash("Minho.Lee123"), State = "Active", RoleId = 3 },
      new Account { Id = 7, Username = "WeiLing.Tan", Password = hasher.ComputeMD5Hash("WeiLing.Tan123"), State = "Active", RoleId = 3 },
      new Account { Id = 8, Username = "Aarav.Sharma", Password = hasher.ComputeMD5Hash("Aarav.Sharma123"), State = "Active", RoleId = 9 },
      new Account { Id = 9, Username = "May.Sukjai", Password = hasher.ComputeMD5Hash("May.Sukjai123"), State = "Active", RoleId = 9 },

      // Team 4: Backend Team
      new Account { Id = 10, Username = "Li.Wang", Password = hasher.ComputeMD5Hash("Li.Wang123"), State = "Active", RoleId = 4 },
      new Account { Id = 11, Username = "Jisoo.Kim", Password = hasher.ComputeMD5Hash("Jisoo.Kim123"), State = "Active", RoleId = 4 },
      new Account { Id = 12, Username = "Rizky.Putra", Password = hasher.ComputeMD5Hash("Rizky.Putra123"), State = "Active", RoleId = 9 },
      new Account { Id = 13, Username = "Linh.Nguyen", Password = hasher.ComputeMD5Hash("Linh.Nguyen123"), State = "Active", RoleId = 9 },

      // Team 5: QA Team
      new Account { Id = 14, Username = "Anan.Chaiyawan", Password = hasher.ComputeMD5Hash("Anan.Chaiyawan123"), State = "Active", RoleId = 5 },
      new Account { Id = 15, Username = "Kelvin.Ng", Password = hasher.ComputeMD5Hash("Kelvin.Ng123"), State = "Active", RoleId = 5 },
      new Account { Id = 16, Username = "Chen.Liu", Password = hasher.ComputeMD5Hash("Chen.Liu123"), State = "Active", RoleId = 9 },
      new Account { Id = 17, Username = "Ashley.Brown", Password = hasher.ComputeMD5Hash("Ashley.Brown123"), State = "Active", RoleId = 9 },

      // Team 6: DevOps Team
      new Account { Id = 18, Username = "Aiko.Nakamura", Password = hasher.ComputeMD5Hash("Aiko.Nakamura123"), State = "Active", RoleId = 6 },
      new Account { Id = 19, Username = "David.Miller", Password = hasher.ComputeMD5Hash("David.Miller123"), State = "Active", RoleId = 6 },
      new Account { Id = 20, Username = "Thao.Pham", Password = hasher.ComputeMD5Hash("Thao.Pham123"), State = "Active", RoleId = 9 },
      new Account { Id = 21, Username = "Yui.Takahashi", Password = hasher.ComputeMD5Hash("Yui.Takahashi123"), State = "Active", RoleId = 9 },

      // Team 7: UI/UX Design Team
      new Account { Id = 22, Username = "Nur.Aisyah", Password = hasher.ComputeMD5Hash("Nur.Aisyah123"), State = "Active", RoleId = 7 },
      new Account { Id = 23, Username = "Kenta.Fujimoto", Password = hasher.ComputeMD5Hash("Kenta.Fujimoto123"), State = "Active", RoleId = 7 },
      new Account { Id = 24, Username = "Myung.Kim", Password = hasher.ComputeMD5Hash("Myung.Kim123"), State = "Active", RoleId = 9 },
      new Account { Id = 25, Username = "Phuc.Tran", Password = hasher.ComputeMD5Hash("Phuc.Tran123"), State = "Active", RoleId = 9 },

      // Team 8: HR Team
      new Account { Id = 26, Username = "Tram.Pham", Password = hasher.ComputeMD5Hash("Tram.Pham123"), State = "Active", RoleId = 8 },
      new Account { Id = 27, Username = "Mei.Zhao", Password = hasher.ComputeMD5Hash("Mei.Zhao123"), State = "Active", RoleId = 8 } 
    );
  }
}
