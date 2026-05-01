using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVotingSystemWebAPIsBackUp.Migrations
{
    /// <inheritdoc />
    public partial class AddVotingRegistrationss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "FacialScanScore",
                table: "VotingRegistrations",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FaceImagePath",
                table: "VotingRegistrations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaceImagePath",
                table: "VotingRegistrations");

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
