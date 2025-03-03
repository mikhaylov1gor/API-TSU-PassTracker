using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_TSU_PassTracker.Migrations
{
    /// <inheritdoc />
    public partial class updateConfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConfirmationType",
                table: "Confirmation",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmationType",
                table: "Confirmation");
        }
    }
}
