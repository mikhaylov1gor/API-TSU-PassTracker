using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TSU_PassTracker.Migrations
{
    /// <inheritdoc />
    public partial class oneConfirmationInRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Confirmation_RequestId",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Confirmation");

            migrationBuilder.CreateTable(
                name: "ConfirmationFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    FileData = table.Column<byte[]>(type: "bytea", nullable: false),
                    ConfirmationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmationFile_Confirmation_ConfirmationId",
                        column: x => x.ConfirmationId,
                        principalTable: "Confirmation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Confirmation_RequestId",
                table: "Confirmation",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationFile_ConfirmationId",
                table: "ConfirmationFile",
                column: "ConfirmationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmationFile");

            migrationBuilder.DropIndex(
                name: "IX_Confirmation_RequestId",
                table: "Confirmation");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "Confirmation",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Confirmation",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Confirmation_RequestId",
                table: "Confirmation",
                column: "RequestId");
        }
    }
}
