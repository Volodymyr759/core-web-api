using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class CreateCompanyService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyServices", x => x.Id);
                });

            migrationBuilder.Sql("INSERT INTO dbo.CompanyServices (Title, Description, ImageUrl, IsActive) VALUES ('Lorem Ipsum', 'Voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi', 'https://somewhere.com/1', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.CompanyServices (Title, Description, ImageUrl, IsActive) VALUES ('Sed ut perspiciatis', 'Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore', 'https://somewhere.com/2', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.CompanyServices (Title, Description, ImageUrl, IsActive) VALUES ('Magni Dolores', 'Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia', 'https://somewhere.com/3', 1)");
            migrationBuilder.Sql("INSERT INTO dbo.CompanyServices (Title, Description, ImageUrl, IsActive) VALUES ('Nemo Enim', 'At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis', 'https://somewhere.com/4', 1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyServices");
        }
    }
}
