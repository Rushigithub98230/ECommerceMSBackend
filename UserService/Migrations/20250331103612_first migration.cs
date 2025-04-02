using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UserService.Migrations
{
    /// <inheritdoc />
    public partial class firstmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 31, 10, 36, 9, 710, DateTimeKind.Utc).AddTicks(8626), "admin123@yopmail.com.com", "Admin", "User", "$2a$11$LnWxFljr8tkPfC0hxAgVqOWV7ysQXsi9lOxapEddXumWvtCu0FxTW", "9823016913", "admin", null },
                    { 2, new DateTime(2025, 3, 31, 10, 36, 9, 871, DateTimeKind.Utc).AddTicks(7220), "seller123@yopmail.com", "Seller", "User", "$2a$11$61gykdCNPRWUNadWb.8e5.DNAcFe4.K7Y7J3Ur8kZKjELoO59bTJ.", "9823016913", "seller", null },
                    { 3, new DateTime(2025, 3, 31, 10, 36, 10, 33, DateTimeKind.Utc).AddTicks(3843), "customer123@yopmail.com", "Customer", "User", "$2a$11$AEyxBcGFDEbrstTZR5UrCeqzRLblEUZuF0tBJwqwlkEo063SyGUrG", "9823016913", "customer", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
