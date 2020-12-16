using Microsoft.EntityFrameworkCore.Migrations;

namespace TrainingAPI.Migrations
{
    public partial class updateuserunsignedname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnsignedName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnsignedName",
                table: "Users");
        }
    }
}
