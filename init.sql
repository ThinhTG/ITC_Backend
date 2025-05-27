-- Create the database
CREATE DATABASE InterTransConnectDB;
GO

USE InterTransConnectDB;
GO

-- Create Users table
CREATE TABLE [Users] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [UserName] NVARCHAR(256),
    [NormalizedUserName] NVARCHAR(256),
    [Email] NVARCHAR(256),
    [NormalizedEmail] NVARCHAR(256),
    [EmailConfirmed] BIT NOT NULL,
    [PasswordHash] NVARCHAR(MAX),
    [SecurityStamp] NVARCHAR(MAX),
    [ConcurrencyStamp] NVARCHAR(MAX),
    [PhoneNumber] NVARCHAR(MAX),
    [PhoneNumberConfirmed] BIT NOT NULL,
    [TwoFactorEnabled] BIT NOT NULL,
    [LockoutEnd] DATETIMEOFFSET,
    [LockoutEnabled] BIT NOT NULL,
    [AccessFailedCount] INT NOT NULL,
    [RefreshToken] NVARCHAR(MAX),
    [RefreshTokenExpiryTime] DATETIME2,
    [WalletId] UNIQUEIDENTIFIER,
    [orderCode] INT
);

-- Create Roles table
CREATE TABLE [Roles] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Name] NVARCHAR(256),
    [NormalizedName] NVARCHAR(256),
    [ConcurrencyStamp] NVARCHAR(MAX),
    [CreatedTime] DATETIMEOFFSET NOT NULL
);

-- Create UserRoles table
CREATE TABLE [UserRoles] (
    [UserId] UNIQUEIDENTIFIER,
    [RoleId] UNIQUEIDENTIFIER,
    [CreatedTime] DATETIMEOFFSET NOT NULL,
    [DeletedTime] DATETIMEOFFSET,
    [LastUpdatedTime] DATETIMEOFFSET NOT NULL,
    PRIMARY KEY ([UserId], [RoleId]),
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id])
);

-- Create Jobs table
CREATE TABLE [Jobs] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [CompanyName] NVARCHAR(MAX),
    [CompanyDescription] NVARCHAR(MAX),
    [CompanyLogoUrl] NVARCHAR(MAX),
    [ContactEmail] NVARCHAR(MAX),
    [ContactPhone] NVARCHAR(MAX),
    [ContactAddress] NVARCHAR(MAX),
    [CreatedAt] DATETIME2 NOT NULL,
    [CustomerId] UNIQUEIDENTIFIER,
    FOREIGN KEY ([CustomerId]) REFERENCES [Users]([Id])
);

-- Create JobApplications table
CREATE TABLE [JobApplications] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [JobId] UNIQUEIDENTIFIER,
    [InterpreterId] UNIQUEIDENTIFIER,
    [Status] NVARCHAR(MAX) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [LastUpdatedAt] DATETIME2 NOT NULL,
    FOREIGN KEY ([JobId]) REFERENCES [Jobs]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([InterpreterId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

-- Create Wallets table
CREATE TABLE [Wallets] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY
);

-- Create WalletTransaction table
CREATE TABLE [WalletTransaction] (
    [WalletTransactionId] UNIQUEIDENTIFIER PRIMARY KEY,
    [WalletId] UNIQUEIDENTIFIER,
    [Amount] DECIMAL(18,2) NOT NULL,
    [TransactionType] NVARCHAR(MAX) NOT NULL,
    [TransactionStatus] NVARCHAR(MAX) NOT NULL,
    [TransactionDate] NVARCHAR(MAX) NOT NULL,
    [TransactionBalance] NVARCHAR(MAX) NOT NULL,
    [OrderId] INT,
    FOREIGN KEY ([WalletId]) REFERENCES [Wallets]([Id]) ON DELETE CASCADE
);

-- Create Orders table
CREATE TABLE [Orders] (
    [OrderId] INT IDENTITY(1,1) PRIMARY KEY,
    [CustomerId] UNIQUEIDENTIFIER NOT NULL,
    [InterpreterId] UNIQUEIDENTIFIER NOT NULL,
    [JobId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedAt] DATETIME2 NOT NULL,
    [EndTime] DATETIME2 NOT NULL,
    FOREIGN KEY ([CustomerId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([InterpreterId]) REFERENCES [Users]([Id]),
    FOREIGN KEY ([JobId]) REFERENCES [Jobs]([Id]) ON DELETE CASCADE
);

-- Create other identity-related tables
CREATE TABLE [UserClaims] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [ClaimType] NVARCHAR(MAX),
    [ClaimValue] NVARCHAR(MAX),
    [CreatedTime] DATETIMEOFFSET NOT NULL,
    [DeletedTime] DATETIMEOFFSET,
    [LastUpdatedTime] DATETIMEOFFSET NOT NULL,
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserLogins] (
    [LoginProvider] NVARCHAR(450),
    [ProviderKey] NVARCHAR(450),
    [ProviderDisplayName] NVARCHAR(MAX),
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [CreatedTime] DATETIMEOFFSET NOT NULL,
    [DeletedTime] DATETIMEOFFSET,
    [LastUpdatedTime] DATETIMEOFFSET NOT NULL,
    PRIMARY KEY ([LoginProvider], [ProviderKey]),
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [UserTokens] (
    [UserId] UNIQUEIDENTIFIER NOT NULL,
    [LoginProvider] NVARCHAR(450) NOT NULL,
    [Name] NVARCHAR(450) NOT NULL,
    [Value] NVARCHAR(MAX),
    [CreatedTime] DATETIMEOFFSET NOT NULL,
    [DeletedTime] DATETIMEOFFSET,
    [LastUpdatedTime] DATETIMEOFFSET NOT NULL,
    PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [RoleClaims] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [RoleId] UNIQUEIDENTIFIER NOT NULL,
    [ClaimType] NVARCHAR(MAX),
    [ClaimValue] NVARCHAR(MAX),
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]) ON DELETE CASCADE
);

GO 