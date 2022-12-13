using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class UpdateAspNetUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("AvatarUrl", "AspNetUsers", type: "nvarchar(max)", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("AvatarUrl", "AspNetUsers");
        }
    }
}
