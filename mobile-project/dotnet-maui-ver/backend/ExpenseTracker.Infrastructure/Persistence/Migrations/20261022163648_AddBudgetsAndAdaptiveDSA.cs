using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetsAndAdaptiveDSA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "budgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PeriodType = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "budget_allocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CurrentSpent = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AlertThresholdPercent = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 80m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_budget_allocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_budget_allocations_budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_budget_allocations_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "daily_spending_allowances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DailyAllowanceAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ActualSpentToday = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RemainingAllowanceToday = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VelocityIndex = table.Column<decimal>(type: "numeric(8,4)", nullable: false, defaultValue: 1.0m),
                    OutOfMoneyForecastDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_daily_spending_allowances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_daily_spending_allowances_budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "budgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_budget_allocations_BudgetId_CategoryId",
                table: "budget_allocations",
                columns: new[] { "BudgetId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_budget_allocations_CategoryId",
                table: "budget_allocations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_budgets_UserId_StartDate_EndDate",
                table: "budgets",
                columns: new[] { "UserId", "StartDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_budgets_UserId_Status",
                table: "budgets",
                columns: new[] { "UserId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_daily_spending_allowances_BudgetId_Date",
                table: "daily_spending_allowances",
                columns: new[] { "BudgetId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "budget_allocations");

            migrationBuilder.DropTable(
                name: "daily_spending_allowances");

            migrationBuilder.DropTable(
                name: "budgets");
        }
    }
}
