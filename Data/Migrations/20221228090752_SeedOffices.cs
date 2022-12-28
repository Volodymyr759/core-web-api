using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedOffices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.Offices (Name, Description, Address, Latitude, Longitude, CountryId) VALUES ('Main office', 'Test description 1', 'Test address 1', 1.111111, 2.222221, 4)");
            migrationBuilder.Sql("INSERT INTO dbo.Offices (Name, Description, Address, Latitude, Longitude, CountryId) VALUES ('Dev office 1', 'Test description 11', 'Test address 11', 1.111112, 2.222222, 4)");
            migrationBuilder.Sql("INSERT INTO dbo.Offices (Name, Description, Address, Latitude, Longitude, CountryId) VALUES ('Dev office 2', 'Test description 22', 'Test address 22', 1.111115, 2.22225, 4)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Offices");
        }
    }
}
