CREATE TABLE [025_Courts] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_025_Courts] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [025_Matches] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [IsRanked] bit NOT NULL,
    [ChallengeId] int NULL,
    [MatchFormat] int NOT NULL,
    [Team1_Player1Id] int NOT NULL,
    [Team1_Player2Id] int NULL,
    [Team2_Player1Id] int NOT NULL,
    [Team2_Player2Id] int NULL,
    [WinningSide] int NOT NULL,
    CONSTRAINT [PK_025_Matches] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [025_Members] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [JoinDate] datetime2 NOT NULL,
    [RankLevel] float NOT NULL,
    [IsActive] bit NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NULL,
    [TotalMatches] int NOT NULL,
    [WinMatches] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_025_Members] PRIMARY KEY ([Id])
);
GO


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
GO


CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO


CREATE TABLE [025_Bookings] (
    [Id] int NOT NULL IDENTITY,
    [CourtId] int NOT NULL,
    [MemberId] int NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [Notes] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_025_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_025_Bookings_025_Courts_CourtId] FOREIGN KEY ([CourtId]) REFERENCES [025_Courts] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_025_Bookings_025_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [025_Members] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [025_Challenges] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [ChallengeType] int NOT NULL,
    [MatchFormat] int NOT NULL,
    [GameMode] int NOT NULL,
    [Status] int NOT NULL,
    [MaxParticipants] int NULL,
    [EntryFee] decimal(18,2) NOT NULL,
    [PrizeAmount] decimal(18,2) NOT NULL,
    [CreatedBy] int NOT NULL,
    [WinnerId] int NULL,
    [ScheduledDate] datetime2 NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ModifiedDate] datetime2 NULL,
    CONSTRAINT [PK_025_Challenges] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_025_Challenges_025_Members_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [025_Members] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_025_Challenges_025_Members_WinnerId] FOREIGN KEY ([WinnerId]) REFERENCES [025_Members] ([Id])
);
GO


CREATE TABLE [025_WalletTransactions] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] int NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Type] int NOT NULL,
    [Category] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_025_WalletTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_025_WalletTransactions_025_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [025_Members] ([Id]) ON DELETE SET NULL
);
GO


CREATE TABLE [Wallets] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] int NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wallets_025_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [025_Members] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO


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
    CONSTRAINT [FK_025_Participants_025_Challenges_ChallengeId] FOREIGN KEY ([ChallengeId]) REFERENCES [025_Challenges] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_025_Participants_025_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [025_Members] ([Id])
);
GO


CREATE INDEX [IX_025_Bookings_CourtId] ON [025_Bookings] ([CourtId]);
GO


CREATE INDEX [IX_025_Bookings_MemberId] ON [025_Bookings] ([MemberId]);
GO


CREATE INDEX [IX_025_Challenges_CreatedBy] ON [025_Challenges] ([CreatedBy]);
GO


CREATE INDEX [IX_025_Challenges_WinnerId] ON [025_Challenges] ([WinnerId]);
GO


CREATE INDEX [IX_025_Participants_ChallengeId] ON [025_Participants] ([ChallengeId]);
GO


CREATE INDEX [IX_025_Participants_MemberId] ON [025_Participants] ([MemberId]);
GO


CREATE INDEX [IX_025_WalletTransactions_MemberId] ON [025_WalletTransactions] ([MemberId]);
GO


CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO


CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO


CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO


CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO


CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO


CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO


CREATE UNIQUE INDEX [IX_Wallets_MemberId] ON [Wallets] ([MemberId]);
GO


