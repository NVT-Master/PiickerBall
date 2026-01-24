using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPrizeAndStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "025_Challenges",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "PrizeAmount",
                table: "025_Challenges",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "WinnerId",
                table: "025_Challenges",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrizeAmount",
                table: "025_Challenges");

            migrationBuilder.DropColumn(
                name: "WinnerId",
                table: "025_Challenges");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "025_Challenges",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
