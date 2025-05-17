using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultancyApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClientCredentialTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ri",
                table: "Consumptions",
                newName: "ReactiveInductiveRatio");

            migrationBuilder.RenameColumn(
                name: "Rc",
                table: "Consumptions",
                newName: "ReactiveInductiveOutRatio");

            migrationBuilder.RenameColumn(
                name: "Cn",
                table: "Consumptions",
                newName: "ReactiveInductiveOut");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndexDate",
                table: "EndOfMonthEndexes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Etso",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupInfo",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "MeterPointOwnerAssignDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinCapasitiveRate",
                table: "Customers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinInductiveRate",
                table: "Customers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriberMultiplierChangeDate",
                table: "Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndexDate",
                table: "CurrentEndexes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProfileDate",
                table: "Consumptions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<decimal>(
                name: "ActiveConsumption",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActiveGeneration",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalConsumption",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AdditionalGeneration",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Multiplier",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReactiveCapacitive",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReactiveCapacitiveOut",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReactiveCapacitiveOutRatio",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReactiveCapacitiveRatio",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReactiveInductive",
                table: "Consumptions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Consumptions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientCredentials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortalUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PortalPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppPassword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCredentials", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientCredentials");

            migrationBuilder.DropColumn(
                name: "Etso",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "GroupInfo",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MeterPointOwnerAssignDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MinCapasitiveRate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MinInductiveRate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "SubscriberMultiplierChangeDate",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ActiveConsumption",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ActiveGeneration",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "AdditionalConsumption",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "AdditionalGeneration",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "Multiplier",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ReactiveCapacitive",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ReactiveCapacitiveOut",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ReactiveCapacitiveOutRatio",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ReactiveCapacitiveRatio",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "ReactiveInductive",
                table: "Consumptions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Consumptions");

            migrationBuilder.RenameColumn(
                name: "ReactiveInductiveRatio",
                table: "Consumptions",
                newName: "Ri");

            migrationBuilder.RenameColumn(
                name: "ReactiveInductiveOutRatio",
                table: "Consumptions",
                newName: "Rc");

            migrationBuilder.RenameColumn(
                name: "ReactiveInductiveOut",
                table: "Consumptions",
                newName: "Cn");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndexDate",
                table: "EndOfMonthEndexes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndexDate",
                table: "CurrentEndexes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ProfileDate",
                table: "Consumptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
