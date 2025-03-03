using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TSU_PassTracker.Migrations
{
    /// <inheritdoc />
    public partial class updateConfirmation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Confirmation",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Confirmation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id");
        }
    }
}
