using Microsoft.EntityFrameworkCore;
using TaskManagementApis.Models;
using TaskManagementApis.Data.Configurations;

namespace TaskManagementApis.Data;

public class AppDbContext : DbContext {
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

  public DbSet<Team> Teams { get; set; }
  public DbSet<Employee> Employees { get; set; }
  public DbSet<Leader> Leaders { get; set; }
  public DbSet<Role> Roles { get; set; }
  public DbSet<Account> Accounts { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<TASK> Tasks { get; set; }
  public DbSet<TaskState> TaskStates { get; set; }
  public DbSet<Comment> Comments { get; set; }
  public DbSet<RefreshToken> RefreshTokens { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);

    // Configure Entities properties, relationships and add sample data
    modelBuilder.ApplyConfiguration(new TeamConfiguration());
    modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
    modelBuilder.ApplyConfiguration(new LeaderConfiguration());
    modelBuilder.ApplyConfiguration(new RoleConfiguration());
    modelBuilder.ApplyConfiguration(new AccountConfiguration());
    modelBuilder.ApplyConfiguration(new CategoryConfiguration());
    modelBuilder.ApplyConfiguration(new TASKConfiguration());
    modelBuilder.ApplyConfiguration(new TaskStateConfiguration());
    modelBuilder.ApplyConfiguration(new CommentConfiguration());
    modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
  }
}
