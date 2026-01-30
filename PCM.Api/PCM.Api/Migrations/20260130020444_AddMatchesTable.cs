using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Matches table
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Matches]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [dbo].[025_Matches] (
                        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Date] DATETIME2 NOT NULL,
                        [IsRanked] BIT NOT NULL,
                        [ChallengeId] INT NULL,
                        [MatchFormat] INT NOT NULL,
                        [Team1_Player1Id] INT NOT NULL,
                        [Team1_Player2Id] INT NULL,
                        [Team2_Player1Id] INT NOT NULL,
                        [Team2_Player2Id] INT NULL,
                        [WinningSide] INT NOT NULL
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Matches]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [dbo].[025_Matches];
                END
            ");
        }
    }
}
