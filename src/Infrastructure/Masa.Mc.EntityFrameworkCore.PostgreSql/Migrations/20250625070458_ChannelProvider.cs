using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Masa.Mc.EntityFrameworkCore.PostgreSql.Migrations
{
    public partial class ChannelProvider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "Channels",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Channels");
        }
    }
}
