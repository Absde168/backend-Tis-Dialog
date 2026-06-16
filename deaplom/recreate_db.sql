USE diplombd;
GO

-- ============================================================
-- ШАГ 1: УДАЛЕНИЕ СТАРЫХ ТАБЛИЦ
-- Порядок важен: сначала дочерние, потом родительские
-- ============================================================

IF OBJECT_ID('dbo.SystemNotifications', 'U') IS NOT NULL DROP TABLE dbo.SystemNotifications;
IF OBJECT_ID('dbo.ChatMessages',        'U') IS NOT NULL DROP TABLE dbo.ChatMessages;
IF OBJECT_ID('dbo.PaymentHistory',      'U') IS NOT NULL DROP TABLE dbo.PaymentHistory;
IF OBJECT_ID('dbo.ConnectionHistory',   'U') IS NOT NULL DROP TABLE dbo.ConnectionHistory;
IF OBJECT_ID('dbo.Users',               'U') IS NOT NULL DROP TABLE dbo.Users;

PRINT 'Старые таблицы удалены.';
GO

-- ============================================================
-- ШАГ 2: СОЗДАНИЕ ТАБЛИЦ
-- ============================================================

-- ------------------------------------------------------------
-- Users
-- ------------------------------------------------------------
CREATE TABLE dbo.Users
(
    Id               INT           NOT NULL IDENTITY(1,1),
    Login            NVARCHAR(100) NOT NULL,
    PasswordHash     NVARCHAR(256) NOT NULL,
    FirstName        NVARCHAR(100) NOT NULL,
    LastName         NVARCHAR(100) NOT NULL,
    Email            NVARCHAR(200) NOT NULL,
    Phone            NVARCHAR(20)      NULL,
    RegistrationDate DATETIME      NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT PK_Users       PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Login UNIQUE      (Login)
);
GO

-- ------------------------------------------------------------
-- ConnectionHistory
-- ------------------------------------------------------------
CREATE TABLE dbo.ConnectionHistory
(
    Id              INT      NOT NULL IDENTITY(1,1),
    UserId          INT      NOT NULL,
    ConnectedAt     DATETIME NOT NULL DEFAULT GETUTCDATE(),
    DisconnectedAt  DATETIME     NULL,
    DataUsedBytes   BIGINT   NOT NULL DEFAULT 0,

    CONSTRAINT PK_ConnectionHistory PRIMARY KEY (Id),

    CONSTRAINT FK_ConnectionHistory_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------
-- PaymentHistory
-- ------------------------------------------------------------
CREATE TABLE dbo.PaymentHistory
(
    Id            INT             NOT NULL IDENTITY(1,1),
    UserId        INT             NOT NULL,
    Amount        DECIMAL(18, 2)  NOT NULL,
    PaymentDate   DATETIME        NOT NULL DEFAULT GETUTCDATE(),
    PaymentMethod NVARCHAR(50)    NOT NULL DEFAULT N'Card',
    Status        NVARCHAR(50)    NOT NULL DEFAULT N'Success',
    TransactionId NVARCHAR(100)   NOT NULL DEFAULT CAST(NEWID() AS NVARCHAR(100)),

    CONSTRAINT PK_PaymentHistory PRIMARY KEY (Id),

    CONSTRAINT FK_PaymentHistory_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------
-- ChatMessages
-- ------------------------------------------------------------
CREATE TABLE dbo.ChatMessages
(
    Id         INT           NOT NULL IDENTITY(1,1),
    UserId     INT           NOT NULL,
    Content    NVARCHAR(MAX) NOT NULL DEFAULT N'',
    SenderType NVARCHAR(50)  NOT NULL DEFAULT N'User',
    Timestamp  DATETIME      NOT NULL DEFAULT GETUTCDATE(),

    CONSTRAINT PK_ChatMessages PRIMARY KEY (Id),

    CONSTRAINT FK_ChatMessages_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users (Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
GO

-- ------------------------------------------------------------
-- SystemNotifications
-- ------------------------------------------------------------
CREATE TABLE dbo.SystemNotifications
(
    Id        INT           NOT NULL IDENTITY(1,1),
    Title     NVARCHAR(255) NOT NULL DEFAULT N'',
    Body      NVARCHAR(MAX) NOT NULL DEFAULT N'',
    Type      NVARCHAR(50)  NOT NULL DEFAULT N'Info',
    CreatedAt DATETIME      NOT NULL DEFAULT GETUTCDATE(),
    IsRead    BIT           NOT NULL DEFAULT 0,
    UserId    INT               NULL,

    CONSTRAINT PK_SystemNotifications PRIMARY KEY (Id),

    CONSTRAINT FK_SystemNotifications_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users (Id)
        ON DELETE SET NULL
        ON UPDATE CASCADE
);
GO

PRINT 'Все таблицы успешно созданы со связями.';
GO
