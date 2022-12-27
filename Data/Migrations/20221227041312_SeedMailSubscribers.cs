using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedMailSubscribers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscribers (Email, IsSubscribed, MailSubscriptionId) VALUES ('test1@gmail.com', 1, 1)");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscribers (Email, IsSubscribed, MailSubscriptionId) VALUES ('test2@gmail.com', 1, 1)");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscribers (Email, IsSubscribed, MailSubscriptionId) VALUES ('test3@gmail.com', 1, 1)");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscribers (Email, IsSubscribed, MailSubscriptionId) VALUES ('test4@gmail.com', 1, 2)");
            migrationBuilder.Sql("INSERT INTO dbo.MailSubscribers (Email, IsSubscribed, MailSubscriptionId) VALUES ('test5@gmail.com', 1, 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.MailSubscribers");
        }
    }
}
