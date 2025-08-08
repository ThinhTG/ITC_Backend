using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddFreeJobPostsUsed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FreeJobPostsUsed",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FreeJobPostsUsed",
                table: "Users");
        }
    }
}
