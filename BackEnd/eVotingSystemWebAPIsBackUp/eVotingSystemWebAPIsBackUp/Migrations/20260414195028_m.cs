using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVotingSystemWebAPIsBackUp.Migrations
{
    /// <inheritdoc />
    public partial class m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FaceImagePath",
                table: "VotingRegistrations",
                newName: "FacialScanPath");

            migrationBuilder.AlterColumn<decimal>(
                name: "FacialScanScore",
                table: "VotingRegistrations",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FacialScanPath",
                table: "VotingRegistrations",
                newName: "FaceImagePath");

            migrationBuilder.AlterColumn<decimal>(
                name: "FacialScanScore",
                table: "VotingRegistrations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);
        }
    }
}
