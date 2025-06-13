using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class BPDVCertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslatorCertificate",
                columns: table => new
                {
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Experience = table.Column<int>(type: "int", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CvFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationForm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationLanguage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatorCertificate", x => x.ApplicationUserId);
                    table.ForeignKey(
                        name: "FK_TranslatorCertificate_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslatorCertificate");
        }
    }
}
