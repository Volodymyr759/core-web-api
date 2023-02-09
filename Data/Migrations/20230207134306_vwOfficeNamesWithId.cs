using Microsoft.EntityFrameworkCore.Migrations;

namespace CoreWebApi.Data.Migrations
{
    public partial class vwOfficeNamesWithId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR ALTER VIEW [dbo].[vwOfficeNamesWithId] AS SELECT Id, Name  FROM [dbo].Offices");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"drop view vwOfficeNamesWithId;");
        }
    }
}
