using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVotingSystemWebAPIsBackUp.Migrations
{
    /// <inheritdoc />
    public partial class aaaas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "VotingRegistrations",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "VotingRegistrations");
        }
    }
}
