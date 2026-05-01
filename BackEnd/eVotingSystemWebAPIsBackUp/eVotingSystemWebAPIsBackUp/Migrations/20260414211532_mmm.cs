using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVotingSystemWebAPIsBackUp.Migrations
{
    /// <inheritdoc />
    public partial class mmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "AddressVerified",
                table: "VotingRegistrations",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminComment",
                table: "VotingRegistrations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ResidentialAddress",
                table: "VotingRegistrations",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressVerified",
                table: "VotingRegistrations");

            migrationBuilder.DropColumn(
                name: "AdminComment",
                table: "VotingRegistrations");

            migrationBuilder.DropColumn(
                name: "ResidentialAddress",
                table: "VotingRegistrations");

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
    }
}
