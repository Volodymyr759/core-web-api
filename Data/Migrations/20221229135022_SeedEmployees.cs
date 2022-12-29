using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class SeedEmployees : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO dbo.Employees (FullName, Email, Position, Description,  AvatarUrl, OfficeId) VALUES ('John Done', 'john@gmail.com', 'CEO', 'CEO description', 'https://www.somewhere.com/1', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.Employees (FullName, Email, Position, Description,  AvatarUrl, OfficeId) VALUES ('Jane Dane', 'jane@gmail.com', 'Developer', 'Developer description', 'https://www.somewhere.com/2', 2)");
            migrationBuilder.Sql("INSERT INTO dbo.Employees (FullName, Email, Position, Description,  AvatarUrl, OfficeId) VALUES ('Jack Dack', 'jack@gmail.com', 'Developer', 'Developer description', 'https://www.somewhere.com/3', 2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM dbo.Employees");
        }
    }
}
