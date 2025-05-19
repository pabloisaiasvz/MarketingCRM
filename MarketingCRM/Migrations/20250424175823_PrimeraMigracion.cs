using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MarketingCRM.Migrations
{
    /// <inheritdoc />
    public partial class PrimeraMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApproverRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproverRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_ApproverRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ApproverRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MinAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: true),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    ApproverRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRules_ApproverRoles_ApproverRoleId",
                        column: x => x.ApproverRoleId,
                        principalTable: "ApproverRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalRules_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ApprovalRules_ProjectTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectProposals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    EstimatedAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EstimatedDuration = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProposals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_ApprovalStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_ProjectTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ProjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectProposals_Users_CreateById",
                        column: x => x.CreateById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectApprovalSteps",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectProposalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproverUserId = table.Column<int>(type: "int", nullable: true),
                    ApproverRoleId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observations = table.Column<string>(type: "varchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectApprovalSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectApprovalSteps_ApprovalStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApprovalStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectApprovalSteps_ApproverRoles_ApproverRoleId",
                        column: x => x.ApproverRoleId,
                        principalTable: "ApproverRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectApprovalSteps_ProjectProposals_ProjectProposalId",
                        column: x => x.ProjectProposalId,
                        principalTable: "ProjectProposals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectApprovalSteps_Users_ApproverUserId",
                        column: x => x.ApproverUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApprovalStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Approved" },
                    { 3, "Rejected" },
                    { 4, "Observed" }
                });

            migrationBuilder.InsertData(
                table: "ApproverRoles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Líder de Área" },
                    { 2, "Gerente" },
                    { 3, "Director" },
                    { 4, "Comité Técnico" }
                });

            migrationBuilder.InsertData(
                table: "Areas",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Finanzas" },
                    { 2, "Tecnología" },
                    { 3, "Recursos Humanos" },
                    { 4, "Operaciones" }
                });

            migrationBuilder.InsertData(
                table: "ProjectTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mejora de Procesos" },
                    { 2, "Innovación y Desarrollo" },
                    { 3, "Infraestructura" },
                    { 4, "Capacitación Interna" }
                });

            migrationBuilder.InsertData(
                table: "ApprovalRules",
                columns: new[] { "Id", "ApproverRoleId", "AreaId", "MaxAmount", "MinAmount", "StepOrder", "TypeId" },
                values: new object[,]
                {
                    { 1L, 1, null, 100000m, 0m, 1, null },
                    { 2L, 2, null, 20000m, 5000m, 2, null },
                    { 3L, 2, 2, 20000m, 0m, 1, 2 },
                    { 4L, 3, null, 0m, 20000m, 3, null },
                    { 5L, 2, 1, 0m, 5000m, 2, 1 },
                    { 6L, 1, null, 10000m, 0m, 1, 2 },
                    { 7L, 1, 2, 10000m, 0m, 4, 1 },
                    { 8L, 2, 2, 30000m, 10000m, 2, null },
                    { 9L, 3, 3, 0m, 30000m, 2, null },
                    { 10L, 4, null, 50000m, 0m, 1, 4 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "RoleId" },
                values: new object[,]
                {
                    { 1, "jferreyra@unaj.com", "José Ferreyra", 2 },
                    { 2, "alucero@unaj.com", "Ana Lucero", 1 },
                    { 3, "gmolinas@unaj.com", "Gonzalo Molinas", 2 },
                    { 4, "lolivera@unaj.com", "Lucas Olivera", 3 },
                    { 5, "dfagundez@unaj.com", "Danilo Fagundez", 4 },
                    { 6, "ggalli@unaj.com", "Gabriel Galli", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRules_ApproverRoleId",
                table: "ApprovalRules",
                column: "ApproverRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRules_AreaId",
                table: "ApprovalRules",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRules_TypeId",
                table: "ApprovalRules",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApprovalSteps_ApproverRoleId",
                table: "ProjectApprovalSteps",
                column: "ApproverRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApprovalSteps_ApproverUserId",
                table: "ProjectApprovalSteps",
                column: "ApproverUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApprovalSteps_ProjectProposalId",
                table: "ProjectApprovalSteps",
                column: "ProjectProposalId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectApprovalSteps_StatusId",
                table: "ProjectApprovalSteps",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_AreaId",
                table: "ProjectProposals",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_CreateById",
                table: "ProjectProposals",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_StatusId",
                table: "ProjectProposals",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProposals_TypeId",
                table: "ProjectProposals",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalRules");

            migrationBuilder.DropTable(
                name: "ProjectApprovalSteps");

            migrationBuilder.DropTable(
                name: "ProjectProposals");

            migrationBuilder.DropTable(
                name: "ApprovalStatuses");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "ProjectTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ApproverRoles");
        }
    }
}
