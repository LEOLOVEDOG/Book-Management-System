using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Management_System_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateSocialLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RememberToken",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "RememberMe",
                table: "Users",
                newName: "IsExternalLogin");

            migrationBuilder.CreateTable(
                name: "UserSocialLogins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProviderId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSocialLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSocialLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSocialLogins_UserId",
                table: "UserSocialLogins",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSocialLogins");

            migrationBuilder.RenameColumn(
                name: "IsExternalLogin",
                table: "Users",
                newName: "RememberMe");

            migrationBuilder.AddColumn<string>(
                name: "RememberToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
