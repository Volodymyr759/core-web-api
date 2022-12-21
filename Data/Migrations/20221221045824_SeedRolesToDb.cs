using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seeding default roles since we have not noone registered user
            migrationBuilder.Sql("INSERT INTO dbo.Roles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('7fb0b5c1-a7d5-49c4-88cf-e72bc4dca2e4', 'Admin', 'ADMIN', 'f7008064-2617-4d1f-9da5-ee1bc4907d13')");
            migrationBuilder.Sql("INSERT INTO dbo.Roles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('ae0435f3-6105-4e40-8a62-9c98e7bde891', 'Manager', 'MANAGER', '36115d94-a8bc-476c-a2e9-12ae3c185038')");
            migrationBuilder.Sql("INSERT INTO dbo.Roles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES ('059775ff-2037-4aa5-adde-15c06fef0ca5', 'Registered', 'REGISTERED', '08509a30-ec89-4a4d-a68d-ab0e2ad2f297')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Default roles can not be removed
        }
    }
}
