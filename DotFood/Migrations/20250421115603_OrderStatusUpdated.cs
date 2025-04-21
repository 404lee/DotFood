using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotFood.Migrations
{
    public partial class OrderStatusUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_OrderStatus_OrderStatusId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatus_OrderId",
                table: "OrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrderStatusId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "OrderStatus",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "OrderStatusId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_OrderId",
                table: "OrderStatus",
                column: "OrderId",
                unique: true,
                filter: "[OrderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UsersId",
                table: "Orders",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_UsersId",
                table: "Orders",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_UsersId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderStatus_OrderId",
                table: "OrderStatus");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UsersId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderStatusId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Orders");

            migrationBuilder.AlterColumn<long>(
                name: "OrderId",
                table: "OrderStatus",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "OrderStatusId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatus_OrderId",
                table: "OrderStatus",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrderStatusId",
                table: "AspNetUsers",
                column: "OrderStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_OrderStatus_OrderStatusId",
                table: "AspNetUsers",
                column: "OrderStatusId",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
