using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment_CS5.Migrations
{
    /// <inheritdoc />
    public partial class _2605Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Delete",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Delete",
                table: "Order");
        }
    }
}
