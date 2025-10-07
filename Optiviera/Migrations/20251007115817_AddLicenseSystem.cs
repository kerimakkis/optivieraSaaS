using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optiviera.Migrations
{
    public partial class AddLicenseSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicenseKey = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    MachineId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    LicenseType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    GracePeriodEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastValidated = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CustomerEmail = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenseHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LicenseId = table.Column<int>(type: "INTEGER", nullable: false),
                    LicenseKey = table.Column<string>(type: "TEXT", maxLength: 25, nullable: false),
                    MachineId = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    ValidationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsValid = table.Column<bool>(type: "INTEGER", nullable: false),
                    ValidationResult = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseHistories_Licenses_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "Licenses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LicenseHistories_LicenseId",
                table: "LicenseHistories",
                column: "LicenseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicenseHistories");

            migrationBuilder.DropTable(
                name: "Licenses");
        }
    }
}
