using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskManagementApis.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<bool>(type: "bit", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: true),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Employees_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Employees_Id",
                        column: x => x.Id,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Employees_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByTeamId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Employees_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Employees_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Teams_CreatedByTeamId",
                        column: x => x.CreatedByTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Leaders",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    Leadership = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leaders", x => new { x.EmployeeId, x.TeamId });
                    table.ForeignKey(
                        name: "FK_Leaders_Employees_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Leaders_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Leaders_Teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByTeamId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: true),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskStates_Employees_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskStates_Employees_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskStates_Teams_CreatedByTeamId",
                        column: x => x.CreatedByTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Accounts_UserId",
                        column: x => x.UserId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Progress = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: true),
                    Due = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ExecutedByTeamId = table.Column<int>(type: "int", nullable: false),
                    ExecutedByEmployeeId = table.Column<int>(type: "int", nullable: false),
                    ParentTaskId = table.Column<int>(type: "int", nullable: true),
                    CurrentStateId = table.Column<int>(type: "int", nullable: false),
                    CreatedByTeamId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    UpdatedById = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Employees_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Employees_ExecutedByEmployeeId",
                        column: x => x.ExecutedByEmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Employees_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskStates_CurrentStateId",
                        column: x => x.CurrentStateId,
                        principalTable: "TaskStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Tasks_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Teams_CreatedByTeamId",
                        column: x => x.CreatedByTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Teams_ExecutedByTeamId",
                        column: x => x.ExecutedByTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    RepliesTo = table.Column<int>(type: "int", nullable: true),
                    CommentedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Accounts_CommentedById",
                        column: x => x.CommentedById,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_RepliesTo",
                        column: x => x.RepliesTo,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedById", "CreatedByTeamId", "Description", "Name", "UpdatedById" },
                values: new object[,]
                {
                    { 1, null, null, null, "Initializing a new project", null },
                    { 2, null, null, null, "Planning", null },
                    { 3, null, null, null, "Design", null },
                    { 4, null, null, null, "Development", null },
                    { 5, null, null, null, "Testing", null },
                    { 6, null, null, null, "Deployment", null },
                    { 7, null, null, null, "Maintenance", null }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Dob", "Email", "Firstname", "Gender", "Lastname", "ManagerId", "Phone", "TeamId", "UpdatedById" },
                values: new object[,]
                {
                    { -1, null, "No Email", "Abstract Employee", null, "Used for assigning tasks that requires many employees to excecute", null, "No phone", null, null },
                    { 1, null, "system.admin@taskmanagement.com", "TaskManagement System Admin", false, null, null, "0137270136", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "RoleName" },
                values: new object[,]
                {
                    { 1, "Describe System Admin role.", "System Admin" },
                    { 2, "Describe Project Manager role.", "Project Manager" },
                    { 3, "Describe Frontend Leader role.", "Frontend Leader" },
                    { 4, "Describe Backend Leader role.", "Backend Leader" },
                    { 5, "Describe QA Leader role.", "QA Leader" },
                    { 6, "Describe DevOps Leader role.", "DevOps Leader" },
                    { 7, "Describe UI/UX Design Leader role.", "UI/UX Design Leader" },
                    { 8, "Describe HR Leader role.", "HR" },
                    { 9, "Describe Employee Leader role.", "Employee" }
                });

            migrationBuilder.InsertData(
                table: "TaskStates",
                columns: new[] { "Id", "CreatedById", "CreatedByTeamId", "Description", "Name", "UpdatedById" },
                values: new object[,]
                {
                    { 1, null, null, "Project is being scoped, discussed, or brainstormed.", "Planning", null },
                    { 2, null, null, "Project has a defined scope, timeline, and resources.", "Planned", null },
                    { 3, null, null, "Project is in the design phase — wireframes, architecture, or UX.", "Designing", null },
                    { 4, null, null, "Design work is completed and ready for review or handoff.", "Designed", null },
                    { 5, null, null, "Project is fully prepared and queued for development.", "Ready For Devs", null },
                    { 6, null, null, "Actively being worked on by a developer or team.", "In Progress", null },
                    { 7, null, null, "Progress is halted due to dependencies, issues, or missing input.", "Blocked", null },
                    { 8, null, null, "Development is complete; task is ready for testing.", "Ready for QA", null },
                    { 9, null, null, "Project is undergoing testing and validation by QA team.", "In QA", null },
                    { 10, null, null, "QA passed; Project is approved for deployment.", "Ready for Release", null },
                    { 11, null, null, "Project is currently being released to staging or production.", "Deploying", null },
                    { 12, null, null, "Project has been successfully released.", "Deployed", null },
                    { 13, null, null, "Post-deployment checks are underway to confirm success.", "Verifying", null },
                    { 14, null, null, "Project has passed all post-deployment checks.", "Verified", null },
                    { 15, null, null, "Project is fully completed and closed.", "Done", null },
                    { 16, null, null, "Project is no longer active but retained for historical reference.", "Archived", null },
                    { 17, null, null, "Task was intentionally stopped and will not be completed.", "Canceled", null },
                    { 18, null, null, "Task awaits sign-off from a stakeholder or reviewer.", "Pending Approval", null },
                    { 19, null, null, "Task is under active evaluation for quality or correctness.", "In Review", null }
                });

            migrationBuilder.InsertData(
                table: "Teams",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { -1, "This team id is used only for tasks assigned for more than a team.", "Abstract Team" },
                    { 1, "Responsible for the entire process of developing products.", "Project Manager Team" },
                    { 2, "Responsible for backend and frontend development", "Frontend Team" },
                    { 3, "Responsible for backend and frontend development", "Backend Team" },
                    { 4, "Handles testing and quality assurance", "QA Team" },
                    { 5, "Manages CI/CD pipelines and deployment", "DevOps Team" },
                    { 6, "Creates user interfaces and experience designs", "UI/UX Design Team" },
                    { 7, "Manange human resources", "HR Team" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Password", "RoleId", "State", "UpdatedById", "Username" },
                values: new object[] { 1, "2259bb6ac53b249da929405e8f5ee733", 1, "Active", null, "sa" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Dob", "Email", "Firstname", "Gender", "Lastname", "ManagerId", "Phone", "TeamId", "UpdatedById" },
                values: new object[,]
                {
                    { 2, null, "emily.johnson@taskmanagement.com", "Emily", false, "Johnson", null, "0123456702", 1, null },
                    { 3, null, "haruki.saito@taskmanagement.com", "Haruki", true, "Saito", null, "0123456703", 1, null },
                    { 4, null, "huong.tran@taskmanagement.com", "Huong", false, "Tran", null, "0123456704", 1, null },
                    { 5, null, "ginny.lewis@taskmanagement.com", "Ginny", false, "Lewis", null, "0123456705", 1, null },
                    { 6, null, "minho.lee@taskmanagement.com", "Minho", true, "Lee", null, "0123456706", 2, null },
                    { 10, null, "li.wang@taskmanagement.com", "Li", true, "Wang", null, "0123456710", 3, null },
                    { 14, null, "anan.chaiyawan@taskmanagement.com", "Anan", true, "Chaiyawan", null, "0123456714", 4, null },
                    { 18, null, "aiko.nakamura@taskmanagement.com", "Aiko", false, "Nakamura", null, "0123456718", 5, null },
                    { 22, null, "nur.aisyah@taskmanagement.com", "Nur", false, "Aisyah", null, "0123456722", 6, null },
                    { 26, null, "tram.pham@taskmanagement.com", "Tram", true, "Pham", null, "0123456726", 7, null }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Password", "RoleId", "State", "UpdatedById", "Username" },
                values: new object[,]
                {
                    { 2, "5bc217716a3034f144e99dd31f524837", 2, "Active", null, "Emily.Johnson" },
                    { 3, "fb1bde4cbc58e0f6f50e57eb83a4289d", 2, "Active", null, "Haruki.Saito" },
                    { 4, "ea8e9f09d10c210bcb142cbdc0de5228", 2, "Active", null, "Huong.Tran" },
                    { 5, "1bcf030ed6b36276fc5af512d96876a3", 2, "Active", null, "Ginny.Lewis" },
                    { 6, "5040354424d0b0d6220138bdaf3a51a7", 3, "Active", null, "Minho.Lee" },
                    { 10, "67767c5a8e9ffbe2924f62854f7c6024", 4, "Active", null, "Li.Wang" },
                    { 14, "a681dc1d69d6145670fa9d31652b6938", 5, "Active", null, "Anan.Chaiyawan" },
                    { 18, "a60595dc6fdd839261ba928693ceafad", 6, "Active", null, "Aiko.Nakamura" },
                    { 22, "31a6ee527514d5be84039a5529bb8493", 7, "Active", null, "Nur.Aisyah" },
                    { 26, "5011d9e38bea853a86cc0bbaa79c4703", 8, "Active", null, "Tram.Pham" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Dob", "Email", "Firstname", "Gender", "Lastname", "ManagerId", "Phone", "TeamId", "UpdatedById" },
                values: new object[,]
                {
                    { 7, null, "weiling.tan@taskmanagement.com", "Wei Ling", false, "Tan", 6, "0123456707", 2, null },
                    { 11, null, "jisoo.kim@taskmanagement.com", "Jisoo", false, "Kim", 10, "0123456711", 3, null },
                    { 15, null, "kelvin.ng@taskmanagement.com", "Kelvin", true, "Ng", 14, "0123456715", 4, null },
                    { 19, null, "david.miller@taskmanagement.com", "David", true, "Miller", 18, "0123456719", 5, null },
                    { 23, null, "kenta.fujimoto@taskmanagement.com", "Kenta", true, "Fujimoto", 22, "0123456723", 6, null },
                    { 27, null, "mei.zhao@taskmanagement.com", "Mei", false, "Zhao", 26, "0123456727", 7, null }
                });

            migrationBuilder.InsertData(
                table: "Leaders",
                columns: new[] { "EmployeeId", "TeamId", "CreatedById", "Leadership" },
                values: new object[,]
                {
                    { 6, 2, null, "Leader" },
                    { 10, 3, null, "Leader" },
                    { 14, 4, null, "Leader" },
                    { 18, 5, null, "Leader" },
                    { 22, 6, null, "Leader" },
                    { 26, 7, null, "Leader" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Password", "RoleId", "State", "UpdatedById", "Username" },
                values: new object[,]
                {
                    { 7, "921b1c9ded555364edc908e0001633f5", 3, "Active", null, "WeiLing.Tan" },
                    { 11, "35e7cb8b2603e3adf8589211e9891a12", 4, "Active", null, "Jisoo.Kim" },
                    { 15, "d97af6e848f042963daeeeeae290943f", 5, "Active", null, "Kelvin.Ng" },
                    { 19, "dbfa1d434ed49efa2ff5f39d0358abd3", 6, "Active", null, "David.Miller" },
                    { 23, "772c2c09d5718afb72790fc25c315c82", 7, "Active", null, "Kenta.Fujimoto" },
                    { 27, "ad80c63a0c88673609cd3f626fa8af72", 8, "Active", null, "Mei.Zhao" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Dob", "Email", "Firstname", "Gender", "Lastname", "ManagerId", "Phone", "TeamId", "UpdatedById" },
                values: new object[,]
                {
                    { 8, null, "aarav.sharma@taskmanagement.com", "Aarav", true, "Sharma", 7, "0123456708", 2, null },
                    { 9, null, "may.sukjai@taskmanagement.com", "May", false, "Sukjai", 7, "0123456709", 2, null },
                    { 12, null, "rizky.putra@taskmanagement.com", "Rizky", true, "Putra", 11, "0123456712", 3, null },
                    { 13, null, "linh.nguyen@taskmanagement.com", "Linh", false, "Nguyen", 11, "0123456713", 3, null },
                    { 16, null, "chen.liu@taskmanagement.com", "Chen", true, "Liu", 15, "0123456716", 4, null },
                    { 17, null, "ashley.brown@taskmanagement.com", "Ashley", false, "Brown", 15, "0123456717", 4, null },
                    { 20, null, "thao.pham@taskmanagement.com", "Thao", false, "Pham", 19, "0123456720", 5, null },
                    { 21, null, "yui.takahashi@taskmanagement.com", "Yui", false, "Takahashi", 19, "0123456721", 5, null },
                    { 24, null, "myung.kim@taskmanagement.com", "Myung", false, "Kim", 23, "0123456724", 6, null },
                    { 25, null, "phuc.tran@taskmanagement.com", "Phuc", true, "Tran", 23, "0123456725", 6, null }
                });

            migrationBuilder.InsertData(
                table: "Leaders",
                columns: new[] { "EmployeeId", "TeamId", "CreatedById", "Leadership" },
                values: new object[,]
                {
                    { 7, 2, null, "Assistant" },
                    { 11, 3, null, "Assistant" },
                    { 15, 4, null, "Assistant" },
                    { 19, 5, null, "Assistant" },
                    { 23, 6, null, "Assistant" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Password", "RoleId", "State", "UpdatedById", "Username" },
                values: new object[,]
                {
                    { 8, "b816aad52c7136f2293b44ed3b89744b", 9, "Active", null, "Aarav.Sharma" },
                    { 9, "3c57c2ee90eb246afb95d5643c7c0610", 9, "Active", null, "May.Sukjai" },
                    { 12, "90f5ae45ee5a13e2f5e0adbc4babb4c0", 9, "Active", null, "Rizky.Putra" },
                    { 13, "d3a8fceda9b1b2e5973bfd7eec6ac8a1", 9, "Active", null, "Linh.Nguyen" },
                    { 16, "17f692bcae0c64c8910a7c915f2cf3c9", 9, "Active", null, "Chen.Liu" },
                    { 17, "1cf648caad21152c13f4e90b6c4cab60", 9, "Active", null, "Ashley.Brown" },
                    { 20, "bbcb8e127a669979aa099002e4c6f8cf", 9, "Active", null, "Thao.Pham" },
                    { 21, "7e2f55efb66cffe432a7505e82e601bd", 9, "Active", null, "Yui.Takahashi" },
                    { 24, "9c3c38ad731b26b0628c9e35ccb38129", 9, "Active", null, "Myung.Kim" },
                    { 25, "e458f6eb31622f20f544060ce8428948", 9, "Active", null, "Phuc.Tran" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UpdatedById",
                table: "Accounts",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedById",
                table: "Categories",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreatedByTeamId",
                table: "Categories",
                column: "CreatedByTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UpdatedById",
                table: "Categories",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentedById",
                table: "Comments",
                column: "CommentedById");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_RepliesTo",
                table: "Comments",
                column: "RepliesTo");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskId",
                table: "Comments",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ManagerId",
                table: "Employees",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_TeamId",
                table: "Employees",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_UpdatedById",
                table: "Employees",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_CreatedById",
                table: "Leaders",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Leaders_TeamId",
                table: "Leaders",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId_TokenHash",
                table: "RefreshTokens",
                columns: new[] { "UserId", "TokenHash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CategoryId",
                table: "Tasks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedById",
                table: "Tasks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByTeamId",
                table: "Tasks",
                column: "CreatedByTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CurrentStateId",
                table: "Tasks",
                column: "CurrentStateId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ExecutedByEmployeeId",
                table: "Tasks",
                column: "ExecutedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ExecutedByTeamId",
                table: "Tasks",
                column: "ExecutedByTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentTaskId",
                table: "Tasks",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UpdatedById",
                table: "Tasks",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStates_CreatedById",
                table: "TaskStates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStates_CreatedByTeamId",
                table: "TaskStates",
                column: "CreatedByTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStates_UpdatedById",
                table: "TaskStates",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Leaders");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "TaskStates");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Teams");
        }
    }
}
