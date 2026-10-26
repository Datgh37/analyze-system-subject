using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseReceiptAndAutoRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auto_categorization_rules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeywordPattern = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TargetCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    PriorityOrder = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsRegex = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auto_categorization_rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_auto_categorization_rules_categories_TargetCategoryId",
                        column: x => x.TargetCategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_auto_categorization_rules_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "receipt_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receipt_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_receipt_items_categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_receipt_items_transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_auto_categorization_rules_TargetCategoryId",
                table: "auto_categorization_rules",
                column: "TargetCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_auto_categorization_rules_UserId_KeywordPattern",
                table: "auto_categorization_rules",
                columns: new[] { "UserId", "KeywordPattern" });

            migrationBuilder.CreateIndex(
                name: "IX_auto_categorization_rules_UserId_PriorityOrder",
                table: "auto_categorization_rules",
                columns: new[] { "UserId", "PriorityOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_receipt_items_CategoryId",
                table: "receipt_items",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_receipt_items_TransactionId",
                table: "receipt_items",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auto_categorization_rules");

            migrationBuilder.DropTable(
                name: "receipt_items");
        }
    }
}
