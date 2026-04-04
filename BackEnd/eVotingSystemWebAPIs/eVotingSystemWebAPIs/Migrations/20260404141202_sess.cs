using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVotingSystemWebAPIs.Migrations
{
    /// <inheritdoc />
    public partial class sess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Voters",
                table: "Voters");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Voters",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voters",
                table: "Voters",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Voters",
                table: "Voters");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Voters");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Voters",
                table: "Voters",
                column: "IdNo");
        }
    }
}
