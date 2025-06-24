using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ITC.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class qqqq : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationLimit",
                table: "SubscriptionPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CommissionFeePercentage",
                table: "SubscriptionPlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsBoosted",
                table: "SubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "JobPostLimit",
                table: "SubscriptionPlans",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ServiceFeePercentage",
                table: "SubscriptionPlans",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationLimit",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "CommissionFeePercentage",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "IsBoosted",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "JobPostLimit",
                table: "SubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "ServiceFeePercentage",
                table: "SubscriptionPlans");
        }
    }
}
