using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment_CS5.Migrations
{
    /// <inheritdoc />
    public partial class _060523_Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PointAdded",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointAdded",
                table: "Order");
        }
    }
}
