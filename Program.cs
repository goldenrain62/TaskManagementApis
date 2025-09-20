using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementApis.Data;
using TaskManagementApis.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<HasherService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add Authentication
builder.Services.AddAuthentication(options => {
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => {
  options.TokenValidationParameters = new TokenValidationParameters {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
    )
  };
});

// Add Authorization
var accountPolicies = new [] { "CanCreateAccount(s)", "CanDeleteAccount(s)" };
var employeePolicies = new [] { "CanCreateEmployee(s)", "CanDeleteEmployee(s)" };
var teamPolicies = new [] { "CanCreateTeam(s)", "CanUpdateTeam(s)", "CanDeleteTeam(s)" };
var leaderPolicies = new [] { "CanCreateLeader(s)", "CanDeleteLeader(s)" };
var rolePolicies = new [] { "CanCreateRole(s)", "CanUpdateRole(s)", "CanDeleteRole(s)" };
var categoryPolicies = new [] { "CanCreateCategory/Categories", "CanUpdateCategory/Categories", "CanDeleteCategory/Categories" };
var taskStatePolicies = new [] { "CanCreateTaskState(s)", "CanUpdateTaskState(s)", "CanDeleteTaskState(s)" };
var taskPolicies = new [] { "CanCreateTask(s)", "CanUpdateTask(s)", "CanDeleteTask(s)" };
var refreshTokenPolicies = new [] { "CanGetTokens", "CanDeleteTokens" };

builder.Services.AddAuthorization(options => {
  // Account model
  foreach (var accountPolicy in accountPolicies) {
    if (accountPolicy == "CanCreateAccount(s)")
      options.AddPolicy(accountPolicy, policy => policy.RequireRole("System Admin", "HR"));
    else options.AddPolicy(accountPolicy, policy => policy.RequireRole("System Admin"));
  }

  // Employee Model
  foreach (var employeePolicy in employeePolicies) {
    if (employeePolicy == "CanCreateEmployee(s)")
      options.AddPolicy(employeePolicy, policy => policy.RequireRole("System Admin", "HR"));
    else options.AddPolicy(employeePolicy, policy => policy.RequireRole("System Admin"));
  }

  // Team Model
  foreach (var teamPolicy in teamPolicies)
    options.AddPolicy(teamPolicy, policy =>
      policy.RequireRole("System Admin"));

  // Leader Model
  foreach (var leaderPolicy in leaderPolicies)
    options.AddPolicy(leaderPolicy, policy =>
      policy.RequireRole("System Admin", "HR"));

  // Role model
  foreach (var rolePolicy in rolePolicies)
    options.AddPolicy(rolePolicy, policy =>
      policy.RequireRole("System Admin"));

  // Category Model
  foreach (var categoryPolicy in categoryPolicies)
    options.AddPolicy(categoryPolicy, policy => policy.RequireRole(
      "System Admin", "Project Manager", "Frontend Leader", "Backend Leader",
      "QA Leader", "DevOps Leader", "UI/UX Design Leader"
    ));

  // TaskState Model
  foreach (var taskStatePolicy in taskStatePolicies) {
    options.AddPolicy(taskStatePolicy, policy => policy.RequireRole(
      "System Admin", "Project Manager", "Frontend Leader", "Backend Leader",
      "QA Leader", "DevOps Leader", "UI/UX Design Leader"
    ));
  }

  // Task Model
  foreach (var taskPolicy in taskPolicies) {
    options.AddPolicy(taskPolicy, policy => policy.RequireRole(
      "System Admin", "Project Manager", "Frontend Leader", "Backend Leader",
      "QA Leader", "DevOps Leader", "UI/UX Design Leader"
    ));
  }

  // RefreshToken model
  foreach (var refreshTokenPolicy in refreshTokenPolicies)
    options.AddPolicy(refreshTokenPolicy, policy =>
      policy.RequireRole("System Admin")); 
});



var app = builder.Build();


if (!app.Environment.IsDevelopment())
  app.UseHttpsRedirection();


// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.Run();


