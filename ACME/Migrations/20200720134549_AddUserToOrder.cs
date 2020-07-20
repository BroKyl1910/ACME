using Microsoft.EntityFrameworkCore.Migrations;

namespace ACME.Migrations
{
    public partial class AddUserToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Orders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserEmail",
                table: "Orders",
                column: "UserEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserEmail",
                table: "Orders",
                column: "UserEmail",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserEmail",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserEmail",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Orders");
        }
    }
}
