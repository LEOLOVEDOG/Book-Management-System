using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Management_System_WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResetRequests_Users_UserId",
                table: "PasswordResetRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests");

            migrationBuilder.RenameTable(
                name: "PasswordResetRequests",
                newName: "PasswordResets");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResetRequests_UserId",
                table: "PasswordResets",
                newName: "IX_PasswordResets_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PasswordResets_Users_UserId",
                table: "PasswordResets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets");

            migrationBuilder.RenameTable(
                name: "PasswordResets",
                newName: "PasswordResetRequests");

            migrationBuilder.RenameIndex(
                name: "IX_PasswordResets_UserId",
                table: "PasswordResetRequests",
                newName: "IX_PasswordResetRequests_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResetRequests",
                table: "PasswordResetRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PasswordResetRequests_Users_UserId",
                table: "PasswordResetRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
