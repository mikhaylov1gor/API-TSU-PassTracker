using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TSU_PassTracker.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "UploadedAt",
                table: "Confirmation");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Confirmation",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Confirmation",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "Confirmation",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation");

            migrationBuilder.DropColumn(
                name: "FileData",
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

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                table: "Confirmation",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "Confirmation",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Confirmation",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Confirmation",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedAt",
                table: "Confirmation",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmation_Request_RequestId",
                table: "Confirmation",
                column: "RequestId",
                principalTable: "Request",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
