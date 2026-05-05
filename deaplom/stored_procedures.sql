USE diplombd;
GO

-- ============================================================
--  ХРАНИМЫЕ ПРОЦЕДУРЫ ДЛЯ БАЗЫ ДАННЫХ diplombd
--  Таблицы: ConnectionHistory | PaymentHistory |
--           ChatMessages       | SystemNotifications
-- ============================================================


-- ============================================================
-- 1. ConnectionHistory
-- ============================================================

-- INSERT
CREATE OR ALTER PROCEDURE sp_Insert_ConnectionHistory
    @UserId          INT,
    @ConnectedAt     DATETIME,
    @DisconnectedAt  DATETIME = NULL,
    @DataUsedBytes   BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ConnectionHistory (UserId, ConnectedAt, DisconnectedAt, DataUsedBytes)
    VALUES (@UserId, @ConnectedAt, @DisconnectedAt, @DataUsedBytes);
    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- UPDATE
CREATE OR ALTER PROCEDURE sp_Update_ConnectionHistory
    @Id              INT,
    @UserId          INT,
    @ConnectedAt     DATETIME,
    @DisconnectedAt  DATETIME = NULL,
    @DataUsedBytes   BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ConnectionHistory
    SET
        UserId         = @UserId,
        ConnectedAt    = @ConnectedAt,
        DisconnectedAt = @DisconnectedAt,
        DataUsedBytes  = @DataUsedBytes
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- DELETE
CREATE OR ALTER PROCEDURE sp_Delete_ConnectionHistory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ConnectionHistory WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- GET ALL
CREATE OR ALTER PROCEDURE sp_GetAll_ConnectionHistory
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ch.Id,
        ch.UserId,
        u.Login        AS UserLogin,
        ch.ConnectedAt,
        ch.DisconnectedAt,
        ch.DataUsedBytes,
        CASE
            WHEN ch.DisconnectedAt IS NOT NULL
            THEN DATEDIFF(SECOND, ch.ConnectedAt, ch.DisconnectedAt)
            ELSE NULL
        END            AS DurationSeconds
    FROM ConnectionHistory ch
    INNER JOIN Users u ON u.Id = ch.UserId
    ORDER BY ch.ConnectedAt DESC;
END;
GO

-- GET BY ID
CREATE OR ALTER PROCEDURE sp_GetById_ConnectionHistory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ch.Id,
        ch.UserId,
        u.Login        AS UserLogin,
        ch.ConnectedAt,
        ch.DisconnectedAt,
        ch.DataUsedBytes,
        CASE
            WHEN ch.DisconnectedAt IS NOT NULL
            THEN DATEDIFF(SECOND, ch.ConnectedAt, ch.DisconnectedAt)
            ELSE NULL
        END            AS DurationSeconds
    FROM ConnectionHistory ch
    INNER JOIN Users u ON u.Id = ch.UserId
    WHERE ch.Id = @Id;
END;
GO


-- ============================================================
-- 2. PaymentHistory
-- ============================================================

-- INSERT
CREATE OR ALTER PROCEDURE sp_Insert_PaymentHistory
    @UserId        INT,
    @Amount        DECIMAL(18,2),
    @PaymentDate   DATETIME      = NULL,
    @PaymentMethod NVARCHAR(50)  = N'Card',
    @Status        NVARCHAR(50)  = N'Success',
    @TransactionId NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO PaymentHistory (UserId, Amount, PaymentDate, PaymentMethod, Status, TransactionId)
    VALUES (
        @UserId,
        @Amount,
        ISNULL(@PaymentDate,   GETUTCDATE()),
        ISNULL(@PaymentMethod, N'Card'),
        ISNULL(@Status,        N'Success'),
        ISNULL(@TransactionId, CAST(NEWID() AS NVARCHAR(100)))
    );
    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- UPDATE
CREATE OR ALTER PROCEDURE sp_Update_PaymentHistory
    @Id            INT,
    @UserId        INT,
    @Amount        DECIMAL(18,2),
    @PaymentDate   DATETIME,
    @PaymentMethod NVARCHAR(50),
    @Status        NVARCHAR(50),
    @TransactionId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE PaymentHistory
    SET
        UserId        = @UserId,
        Amount        = @Amount,
        PaymentDate   = @PaymentDate,
        PaymentMethod = @PaymentMethod,
        Status        = @Status,
        TransactionId = @TransactionId
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- DELETE
CREATE OR ALTER PROCEDURE sp_Delete_PaymentHistory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM PaymentHistory WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- GET ALL
CREATE OR ALTER PROCEDURE sp_GetAll_PaymentHistory
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ph.Id,
        ph.UserId,
        u.Login        AS UserLogin,
        ph.Amount,
        ph.PaymentDate,
        ph.PaymentMethod,
        ph.Status,
        ph.TransactionId
    FROM PaymentHistory ph
    INNER JOIN Users u ON u.Id = ph.UserId
    ORDER BY ph.PaymentDate DESC;
END;
GO

-- GET BY ID
CREATE OR ALTER PROCEDURE sp_GetById_PaymentHistory
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        ph.Id,
        ph.UserId,
        u.Login        AS UserLogin,
        ph.Amount,
        ph.PaymentDate,
        ph.PaymentMethod,
        ph.Status,
        ph.TransactionId
    FROM PaymentHistory ph
    INNER JOIN Users u ON u.Id = ph.UserId
    WHERE ph.Id = @Id;
END;
GO


-- ============================================================
-- 3. ChatMessages
-- ============================================================

-- INSERT
CREATE OR ALTER PROCEDURE sp_Insert_ChatMessage
    @UserId     INT,
    @Content    NVARCHAR(MAX),
    @SenderType NVARCHAR(50) = N'User',
    @Timestamp  DATETIME     = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO ChatMessages (UserId, Content, SenderType, Timestamp)
    VALUES (
        @UserId,
        @Content,
        ISNULL(@SenderType, N'User'),
        ISNULL(@Timestamp,  GETUTCDATE())
    );
    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- UPDATE
CREATE OR ALTER PROCEDURE sp_Update_ChatMessage
    @Id         INT,
    @UserId     INT,
    @Content    NVARCHAR(MAX),
    @SenderType NVARCHAR(50),
    @Timestamp  DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE ChatMessages
    SET
        UserId     = @UserId,
        Content    = @Content,
        SenderType = @SenderType,
        Timestamp  = @Timestamp
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- DELETE
CREATE OR ALTER PROCEDURE sp_Delete_ChatMessage
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM ChatMessages WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- GET ALL
CREATE OR ALTER PROCEDURE sp_GetAll_ChatMessages
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        cm.Id,
        cm.UserId,
        u.Login    AS UserLogin,
        cm.Content,
        cm.SenderType,
        cm.Timestamp
    FROM ChatMessages cm
    INNER JOIN Users u ON u.Id = cm.UserId
    ORDER BY cm.Timestamp DESC;
END;
GO

-- GET BY ID
CREATE OR ALTER PROCEDURE sp_GetById_ChatMessage
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        cm.Id,
        cm.UserId,
        u.Login    AS UserLogin,
        cm.Content,
        cm.SenderType,
        cm.Timestamp
    FROM ChatMessages cm
    INNER JOIN Users u ON u.Id = cm.UserId
    WHERE cm.Id = @Id;
END;
GO


-- ============================================================
-- 4. SystemNotifications
-- ============================================================

-- INSERT
CREATE OR ALTER PROCEDURE sp_Insert_SystemNotification
    @Title     NVARCHAR(255),
    @Body      NVARCHAR(MAX),
    @Type      NVARCHAR(50) = N'Info',
    @CreatedAt DATETIME     = NULL,
    @IsRead    BIT          = 0,
    @UserId    INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO SystemNotifications (Title, Body, Type, CreatedAt, IsRead, UserId)
    VALUES (
        @Title,
        @Body,
        ISNULL(@Type,      N'Info'),
        ISNULL(@CreatedAt, GETUTCDATE()),
        ISNULL(@IsRead,    0),
        @UserId
    );
    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO

-- UPDATE
CREATE OR ALTER PROCEDURE sp_Update_SystemNotification
    @Id        INT,
    @Title     NVARCHAR(255),
    @Body      NVARCHAR(MAX),
    @Type      NVARCHAR(50),
    @CreatedAt DATETIME,
    @IsRead    BIT,
    @UserId    INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE SystemNotifications
    SET
        Title     = @Title,
        Body      = @Body,
        Type      = @Type,
        CreatedAt = @CreatedAt,
        IsRead    = @IsRead,
        UserId    = @UserId
    WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- DELETE
CREATE OR ALTER PROCEDURE sp_Delete_SystemNotification
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM SystemNotifications WHERE Id = @Id;
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO

-- GET ALL
CREATE OR ALTER PROCEDURE sp_GetAll_SystemNotifications
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        sn.Id,
        sn.Title,
        sn.Body,
        sn.Type,
        sn.CreatedAt,
        sn.IsRead,
        sn.UserId,
        u.Login AS UserLogin
    FROM SystemNotifications sn
    LEFT JOIN Users u ON u.Id = sn.UserId
    ORDER BY sn.CreatedAt DESC;
END;
GO

-- GET BY ID
CREATE OR ALTER PROCEDURE sp_GetById_SystemNotification
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        sn.Id,
        sn.Title,
        sn.Body,
        sn.Type,
        sn.CreatedAt,
        sn.IsRead,
        sn.UserId,
        u.Login AS UserLogin
    FROM SystemNotifications sn
    LEFT JOIN Users u ON u.Id = sn.UserId
    WHERE sn.Id = @Id;
END;
GO


PRINT 'Все хранимые процедуры успешно созданы (20 шт.).';
GO
