using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultancyApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppUsername",
                table: "ClientCredentials");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUsername",
                table: "ClientCredentials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
