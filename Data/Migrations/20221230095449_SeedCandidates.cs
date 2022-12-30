using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedCandidates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.Candidates (FullName, Email, Phone, Notes, IsDismissed, JoinedAt, VacancyId) VALUES ('Sindy Crowford', 'sindy@gmail.com', '+1234567891', 'Test note 1', 0, '20221230', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.Candidates (FullName, Email, Phone, Notes, IsDismissed, JoinedAt, VacancyId) VALUES ('Merelin Monroe', 'merelin@gmail.com', '+1234567892', 'Test note 2', 0, '20221231', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.Candidates (FullName, Email, Phone, Notes, IsDismissed, JoinedAt, VacancyId) VALUES ('Julia Roberts', 'julia@gmail.com', '+1234567893', 'Test note 3', 0, '20221229', 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Candidates");
        }
    }
}
