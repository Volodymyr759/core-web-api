using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedCountries : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.Countries (Name, Code) VALUES ('Australia', 'AUS')");
            migrationBuilder.Sql("INSERT INTO dbo.Countries (Name, Code) VALUES ('Ukraine', 'UKR')");
            migrationBuilder.Sql("INSERT INTO dbo.Countries (Name, Code) VALUES ('United States', 'USA')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Countries");
        }
    }
}
