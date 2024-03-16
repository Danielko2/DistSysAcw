using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistSysAcwServer.Migrations
{
    public partial class ArchiveLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint to allow UserApiKey to be nullable
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserApiKey",
                table: "Logs");

            // Alter UserApiKey column to be nullable
            migrationBuilder.AlterColumn<string>(
                name: "UserApiKey",
                table: "Logs",
                type: "nvarchar(450)",
                nullable: true, // change this to true
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: false);

            migrationBuilder.CreateTable(
                name: "LogArchives",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserApiKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogArchives", x => x.LogId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserApiKey",
                table: "Logs",
                column: "UserApiKey",
                principalTable: "Users",
                principalColumn: "ApiKey",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraint before altering the column
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_Users_UserApiKey",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "LogArchives");

            // Reverse the column alteration to be non-nullable
            migrationBuilder.AlterColumn<string>(
                name: "UserApiKey",
                table: "Logs",
                type: "nvarchar(450)",
                nullable: false, // change this back to false
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_Users_UserApiKey",
                table: "Logs",
                column: "UserApiKey",
                principalTable: "Users",
                principalColumn: "ApiKey");
        }
    }
}
