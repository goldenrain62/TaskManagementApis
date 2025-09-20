using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManagementApis.Models;

namespace TaskManagementApis.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee> {
  public void Configure(EntityTypeBuilder<Employee> entity) {
    ConfigureProperties(entity);
    ConfigureRelationships(entity);
    AddSampleData(entity);
  }

  public void ConfigureProperties(EntityTypeBuilder<Employee> entity) {
    entity.HasIndex(e => e.Email)
      .IsUnique();
    entity.Property(e => e.CreatedAt)
      .HasDefaultValueSql("GETDATE()");
    entity.Property(e => e.UpdatedAt)
      .HasDefaultValueSql("GETDATE()");
  }

  public void ConfigureRelationships(EntityTypeBuilder<Employee> entity) {
    // Employees <-> Teams  
    entity.HasOne(e => e.Team)
      .WithMany(t => t.Employees)
      .HasForeignKey(e => e.TeamId)
      .OnDelete(DeleteBehavior.Restrict);

    // Employees <-> Employees
    entity.HasOne(e => e.Manager)
      .WithMany(m => m.Subordinates)
      .HasForeignKey(e => e.ManagerId)
      .OnDelete(DeleteBehavior.Restrict);

    entity.HasOne(e => e.UpdatedByUser) // An Employee is updated data by which user(employee).
      .WithMany()
      .HasForeignKey(e => e.UpdatedById)
      .OnDelete(DeleteBehavior.Restrict);
  }

  public void AddSampleData(EntityTypeBuilder<Employee> entity) {
    entity.HasData(
      // Abstract Employee used for assigning tasks that requires many employees to excecute
      new Employee {
        Id = -1,
        Firstname = "Abstract Employee",
        Lastname = "Used for assigning tasks that requires many employees to excecute",
        Phone = "No phone",
        Email = "No Email"
      },
      // System Admin
      new Employee {
        Id = 1,
        Firstname = "TaskManagement System Admin",
        Gender = false,
        Phone = "0137270136",
        Email = "system.admin@taskmanagement.com"
      },
      // Team 2: Project Manager Team
      new Employee { Id = 2, Firstname = "Emily", Lastname = "Johnson", Gender = false, Phone = "0123456702", Email = "emily.johnson@taskmanagement.com", TeamId = 1 },
      new Employee { Id = 3, Firstname = "Haruki", Lastname = "Saito", Gender = true, Phone = "0123456703", Email = "haruki.saito@taskmanagement.com", TeamId = 1 },
      new Employee { Id = 4, Firstname = "Huong", Lastname = "Tran", Gender = false, Phone = "0123456704", Email = "huong.tran@taskmanagement.com", TeamId = 1 },
      new Employee { Id = 5, Firstname = "Ginny", Lastname = "Lewis", Gender = false, Phone = "0123456705", Email = "ginny.lewis@taskmanagement.com", TeamId = 1 },

      // Team 3: Frontend Team
      new Employee { Id = 6, Firstname = "Minho", Lastname = "Lee", Gender = true, Phone = "0123456706", Email = "minho.lee@taskmanagement.com", TeamId = 2 },
      new Employee { Id = 7, Firstname = "Wei Ling", Lastname = "Tan", Gender = false, Phone = "0123456707", Email = "weiling.tan@taskmanagement.com", TeamId = 2, ManagerId = 6 },
      new Employee { Id = 8, Firstname = "Aarav", Lastname = "Sharma", Gender = true, Phone = "0123456708", Email = "aarav.sharma@taskmanagement.com", TeamId = 2, ManagerId = 7 },
      new Employee { Id = 9, Firstname = "May", Lastname = "Sukjai", Gender = false, Phone = "0123456709", Email = "may.sukjai@taskmanagement.com", TeamId = 2, ManagerId = 7 },

      // Team 4: Backend Team
      new Employee { Id = 10, Firstname = "Li", Lastname = "Wang", Gender = true, Phone = "0123456710", Email = "li.wang@taskmanagement.com", TeamId = 3 },
      new Employee { Id = 11, Firstname = "Jisoo", Lastname = "Kim", Gender = false, Phone = "0123456711", Email = "jisoo.kim@taskmanagement.com", TeamId = 3, ManagerId = 10 },
      new Employee { Id = 12, Firstname = "Rizky", Lastname = "Putra", Gender = true, Phone = "0123456712", Email = "rizky.putra@taskmanagement.com", TeamId = 3, ManagerId = 11 },
      new Employee { Id = 13, Firstname = "Linh", Lastname = "Nguyen", Gender = false, Phone = "0123456713", Email = "linh.nguyen@taskmanagement.com", TeamId = 3, ManagerId = 11 },

      // Team 5: QA Team
      new Employee { Id = 14, Firstname = "Anan", Lastname = "Chaiyawan", Gender = true, Phone = "0123456714", Email = "anan.chaiyawan@taskmanagement.com", TeamId = 4 },
      new Employee { Id = 15, Firstname = "Kelvin", Lastname = "Ng", Gender = true, Phone = "0123456715", Email = "kelvin.ng@taskmanagement.com", TeamId = 4, ManagerId = 14 },
      new Employee { Id = 16, Firstname = "Chen", Lastname = "Liu", Gender = true, Phone = "0123456716", Email = "chen.liu@taskmanagement.com", TeamId = 4, ManagerId = 15 },
      new Employee { Id = 17, Firstname = "Ashley", Lastname = "Brown", Gender = false, Phone = "0123456717", Email = "ashley.brown@taskmanagement.com", TeamId = 4, ManagerId = 15 },

      // Team 6: DevOps Team
      new Employee { Id = 18, Firstname = "Aiko", Lastname = "Nakamura", Gender = false, Phone = "0123456718", Email = "aiko.nakamura@taskmanagement.com", TeamId = 5 },
      new Employee { Id = 19, Firstname = "David", Lastname = "Miller", Gender = true, Phone = "0123456719", Email = "david.miller@taskmanagement.com", TeamId = 5, ManagerId = 18 },
      new Employee { Id = 20, Firstname = "Thao", Lastname = "Pham", Gender = false, Phone = "0123456720", Email = "thao.pham@taskmanagement.com", TeamId = 5, ManagerId = 19 },
      new Employee { Id = 21, Firstname = "Yui", Lastname = "Takahashi", Gender = false, Phone = "0123456721", Email = "yui.takahashi@taskmanagement.com", TeamId = 5, ManagerId = 19 },

      // Team 7: UI/UX Design Team
      new Employee { Id = 22, Firstname = "Nur", Lastname = "Aisyah", Gender = false, Phone = "0123456722", Email = "nur.aisyah@taskmanagement.com", TeamId = 6 },
      new Employee { Id = 23, Firstname = "Kenta", Lastname = "Fujimoto", Gender = true, Phone = "0123456723", Email = "kenta.fujimoto@taskmanagement.com", TeamId = 6, ManagerId = 22 },
      new Employee { Id = 24, Firstname = "Myung", Lastname = "Kim", Gender = false, Phone = "0123456724", Email = "myung.kim@taskmanagement.com", TeamId = 6, ManagerId = 23 },
      new Employee { Id = 25, Firstname = "Phuc", Lastname = "Tran", Gender = true, Phone = "0123456725", Email = "phuc.tran@taskmanagement.com", TeamId = 6, ManagerId = 23 },

      // Team 8: HR Team
      new Employee { Id = 26, Firstname = "Tram", Lastname = "Pham", Gender = true, Phone = "0123456726", Email = "tram.pham@taskmanagement.com", TeamId = 7 },
      new Employee { Id = 27, Firstname = "Mei", Lastname = "Zhao", Gender = false, Phone = "0123456727", Email = "mei.zhao@taskmanagement.com", TeamId = 7, ManagerId = 26 }
    );
  }
}
