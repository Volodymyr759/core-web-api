using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedVacancies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.Vacancies (Title, Description, Previews, IsActive, OfficeId) VALUES ('.Net Developer', 'Test description 1', 0, 1, 1)");
            migrationBuilder.Sql("INSERT INTO dbo.Vacancies (Title, Description, Previews, IsActive, OfficeId) VALUES ('Junior JavaScrip Frontend Developer', 'Test description 2', 0, 1, 1)");
            migrationBuilder.Sql("INSERT INTO dbo.Vacancies (Title, Description, Previews, IsActive, OfficeId) VALUES ('Senior JavaScrip Frontend Developer', 'Test description 3', 0, 1, 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Vacancies");
        }
    }
}
