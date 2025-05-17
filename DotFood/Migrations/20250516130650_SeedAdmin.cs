using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotFood.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a2b9e7f4-1234-4567-890a-123456789abc", null, "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "City", "ConcurrencyStamp", "Country", "Email", "EmailConfirmed", "FullName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b3c0f8a5-5678-4901-9012-abcdef123456", 0, "City", "1c24992e-2a5d-4128-a662-eb4b6806a031", "Country", "admin@example.com", true, "Admin User", false, null, "ADMIN@EXAMPLE.COM", "ADMIN@EXAMPLE.COM", "AQAAAAEAACcQAAAAEN1vyd46yTHzXQu3nraB3TxRY4zrBksasDfY9JhUQnEpafSZn2CtR7W5mD0+7QsLKw==", null, false, "919776ae-72a7-400b-9ca6-0c266e985a91", false, "admin@example.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "a2b9e7f4-1234-4567-890a-123456789abc", "b3c0f8a5-5678-4901-9012-abcdef123456" });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Products_Quantity_NonNegative",
                table: "Products",
                sql: "[Quantity] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Products_Quantity_NonNegative",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "a2b9e7f4-1234-4567-890a-123456789abc", "b3c0f8a5-5678-4901-9012-abcdef123456" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a2b9e7f4-1234-4567-890a-123456789abc");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b3c0f8a5-5678-4901-9012-abcdef123456");
        }
    }
}
