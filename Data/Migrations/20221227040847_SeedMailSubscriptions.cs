using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedMailSubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscriptions (Title, Content) VALUES ('Company News', 'Test conyent 1')");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscriptions (Title, Content) VALUES ('Our Vacancies', 'Test conyent 2')");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscriptions (Title, Content) VALUES ('Other test subscription', 'Test conyent 3')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.MailSubscriptions");
        }
    }
}
