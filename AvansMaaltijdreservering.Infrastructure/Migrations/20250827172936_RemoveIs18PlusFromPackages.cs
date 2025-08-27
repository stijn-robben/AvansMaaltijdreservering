using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvansMaaltijdreservering.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIs18PlusFromPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is18Plus",
                table: "Packages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Is18Plus",
                table: "Packages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
