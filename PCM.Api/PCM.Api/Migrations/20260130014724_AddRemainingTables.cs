using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create 025_Challenges table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Challenges]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [025_Challenges] (
                        [Id] int NOT NULL IDENTITY,
                        [Title] nvarchar(200) NOT NULL,
                        [Description] nvarchar(max) NULL,
                        [EntryFee] decimal(18,2) NOT NULL,
                        [PrizeAmount] decimal(18,2) NOT NULL,
                        [MaxParticipants] int NULL,
                        [MatchFormat] int NOT NULL,
                        [ChallengeType] int NOT NULL,
                        [ScheduledDate] datetime2 NULL,
                        [CreatedBy] int NOT NULL,
                        [Status] int NOT NULL,
                        [WinnerId] int NULL,
                        [CreatedDate] datetime2 NOT NULL,
                        [ModifiedDate] datetime2 NULL,
                        CONSTRAINT [PK_025_Challenges] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_025_Challenges_025_Members_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [025_Members] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_025_Challenges_025_Members_WinnerId] FOREIGN KEY ([WinnerId]) REFERENCES [025_Members] ([Id])
                    );
                    CREATE INDEX [IX_025_Challenges_WinnerId] ON [025_Challenges] ([WinnerId]);
                END
            ");

            // Create 025_Participants table if not exists  
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Participants]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [025_Participants] (
                        [Id] int NOT NULL IDENTITY,
                        [ChallengeId] int NOT NULL,
                        [MemberId] int NOT NULL,
                        [Team] nvarchar(max) NOT NULL,
                        [EntryFeePaid] bit NOT NULL,
                        [EntryFeeAmount] decimal(18,2) NOT NULL,
                        [JoinedAt] datetime2 NOT NULL,
                        [Status] nvarchar(max) NOT NULL,
                        CONSTRAINT [PK_025_Participants] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_025_Participants_025_Challenges_ChallengeId] FOREIGN KEY ([ChallengeId]) REFERENCES [025_Challenges] ([Id]) ON DELETE CASCADE
                    );
                END
            ");

            // Create 025_WalletTransactions table if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_WalletTransactions]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [025_WalletTransactions] (
                        [Id] int NOT NULL IDENTITY,
                        [MemberId] int NULL,
                        [Amount] decimal(18,2) NOT NULL,
                        [Type] int NOT NULL,
                        [Description] nvarchar(max) NULL,
                        [Category] nvarchar(max) NULL,
                        [CreatedBy] nvarchar(max) NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_025_WalletTransactions] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_025_WalletTransactions_025_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [025_Members] ([Id]) ON DELETE SET NULL
                    );
                    CREATE INDEX [IX_025_WalletTransactions_MemberId] ON [025_WalletTransactions] ([MemberId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_News]') AND type in (N'U'))
                BEGIN
                    CREATE TABLE [025_News] (
                        [Id] int NOT NULL IDENTITY,
                        [Title] nvarchar(500) NOT NULL,
                        [Content] nvarchar(max) NOT NULL,
                        [ImageUrl] nvarchar(max) NULL,
                        [IsPinned] bit NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        [UpdatedAt] datetime2 NULL,
                        [AuthorId] nvarchar(max) NULL,
                        CONSTRAINT [PK_025_News] PRIMARY KEY ([Id])
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "025_News");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Participants]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [025_Participants];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_Challenges]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [025_Challenges];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[025_WalletTransactions]') AND type in (N'U'))
                BEGIN
                    DROP TABLE [025_WalletTransactions];
                END
            ");
        }

    }
}
