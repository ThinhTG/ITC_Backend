using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class news : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "TranslatorCertificates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates",
                column: "ApplicationUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates");

            migrationBuilder.DropIndex(
                name: "IX_TranslatorCertificates_ApplicationUserId",
                table: "TranslatorCertificates");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "TranslatorCertificates");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslatorCertificates",
                table: "TranslatorCertificates",
                column: "ApplicationUserId");
        }
    }
}
