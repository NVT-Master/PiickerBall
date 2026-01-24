using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChallenges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "025_Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Config_TargetWins = table.Column<int>(type: "int", nullable: true),
                    CurrentScore_TeamA = table.Column<int>(type: "int", nullable: false),
                    CurrentScore_TeamB = table.Column<int>(type: "int", nullable: false),
                    EntryFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PrizePool = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_025_Challenges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_025_Challenges_025_Members_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "025_Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "025_Participants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChallengeId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntryFeePaid = table.Column<bool>(type: "bit", nullable: false),
                    EntryFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_025_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_025_Participants_025_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "025_Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_025_Participants_025_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "025_Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_025_Challenges_CreatedBy",
                table: "025_Challenges",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_025_Participants_ChallengeId",
                table: "025_Participants",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_025_Participants_MemberId",
                table: "025_Participants",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "025_Participants");

            migrationBuilder.DropTable(
                name: "025_Challenges");
        }
    }
}
