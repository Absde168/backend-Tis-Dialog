USE diplombd;
GO

BEGIN TRANSACTION;

BEGIN TRY

    -- =========================================================
    -- ШАГ 1: Очистка таблиц (порядок важен из-за FK)
    -- =========================================================
    DELETE FROM dbo.SystemNotifications;
    DELETE FROM dbo.ChatMessages;
    DELETE FROM dbo.PaymentHistory;
    DELETE FROM dbo.ConnectionHistory;
    DELETE FROM dbo.Users;

    -- Сброс счётчиков IDENTITY, чтобы Id снова начинался с 1
    DBCC CHECKIDENT ('dbo.SystemNotifications', RESEED, 0);
    DBCC CHECKIDENT ('dbo.ChatMessages',        RESEED, 0);
    DBCC CHECKIDENT ('dbo.PaymentHistory',      RESEED, 0);
    DBCC CHECKIDENT ('dbo.ConnectionHistory',   RESEED, 0);
    DBCC CHECKIDENT ('dbo.Users',               RESEED, 0);

    -- =========================================================
    -- ШАГ 2: Вставка данных
    -- =========================================================

    -- Users
    -- Пароль для всех тестовых пользователей: Test@1234
    -- PasswordHash сгенерирован через BCrypt.Net (cost=11)
    INSERT INTO dbo.Users (Login, PasswordHash, FirstName, LastName, Email, Phone, RegistrationDate)
    VALUES
        ('ivanov_a',  '$2a$11$95ZiskQG6W3tGZ6eeL7TfuRz6Oq9vvnF/C/Pg61imN8zrPODFnFJi', N'Алексей', N'Иванов',  'ivanov@mail.ru',    '+79161234567', '20240110 09:00:00'),
        ('petrova_m', '$2a$11$BNmGIpJSsDnhaAZHR00VHOx86NGdtRv2Ebh5B1TadQ33cX134cGry', N'Мария',   N'Петрова', 'petrova@mail.ru',   '+79162345678', '20240215 10:30:00'),
        ('sidorov_n', '$2a$11$RMmQY2OfFhRhUy/03B4Q7eutgBhQtdxQG.eTOQgfKzLfd6r1FVSkK', N'Николай', N'Сидоров', 'sidorov@gmail.com', '+79163456789', '20240320 11:45:00'),
        ('kozlova_e', '$2a$11$xHBvhiOBTPvOq3ih5bJRu.P.XJojVeY7/uvK5xmkRhdb2hHZ5Rax.', N'Елена',   N'Козлова', 'kozlova@yandex.ru', '+79164567890', '20240405 08:15:00'),
        ('morozov_d', '$2a$11$5A21.ky3Mwm6ei850BA7neD8PUdQF7NZoaVWRa6n2y06H6EgLjxKa', N'Дмитрий', N'Морозов', 'morozov@mail.ru',   '+79165678901', '20240512 14:00:00');

    -- ConnectionHistory
    INSERT INTO dbo.ConnectionHistory (UserId, ConnectedAt, DisconnectedAt, DataUsedBytes)
    VALUES
        (1, '20250110 10:00:00', '20250110 11:30:00', 104857600),
        (2, '20250111 12:00:00', '20250111 13:15:00', 52428800),
        (3, '20250112 09:30:00', '20250112 10:00:00', 20971520),
        (4, '20250113 14:00:00', NULL,                10485760),
        (5, '20250114 16:45:00', '20250114 17:30:00', 314572800);

    -- PaymentHistory
    INSERT INTO dbo.PaymentHistory (UserId, Amount, PaymentDate, PaymentMethod, Status, TransactionId)
    VALUES
        (1,  500.00, '20250105 10:00:00', N'Card', N'Success', N'TXN-000001'),
        (2, 1000.00, '20250108 11:30:00', N'Card', N'Success', N'TXN-000002'),
        (3,  250.00, '20250110 09:15:00', N'SBP',  N'Success', N'TXN-000003'),
        (4,  750.00, '20250112 13:00:00', N'Card', N'Pending', N'TXN-000004'),
        (5, 1500.00, '20250115 17:00:00', N'Cash', N'Success', N'TXN-000005');

    -- ChatMessages
    INSERT INTO dbo.ChatMessages (UserId, Content, SenderType, Timestamp)
    VALUES
        (1, N'Добрый день! Как настроить VPN-соединение?',          N'User', '20250110 10:05:00'),
        (2, N'Подскажите, почему интернет работает медленно?',      N'User', '20250111 12:10:00'),
        (3, N'Хочу сменить тарифный план. Что посоветуете?',        N'User', '20250112 09:35:00'),
        (4, N'Когда будет восстановлено соединение в моём районе?', N'User', '20250113 14:05:00'),
        (5, N'Спасибо за быстрый ответ и помощь!',                  N'User', '20250114 16:50:00');

    -- SystemNotifications
    INSERT INTO dbo.SystemNotifications (Title, Body, Type, CreatedAt, IsRead, UserId)
    VALUES
        (N'Плановые работы',      N'С 02:00 до 04:00 проводятся плановые технические работы.', N'Warning', '20250109 20:00:00', 0, NULL),
        (N'Новый тариф',          N'Теперь доступен тариф Безлимит 1 Гбит/с. Подключите!',    N'Info',    '20250110 08:00:00', 0, 1),
        (N'Оплата получена',      N'Ваш платёж на сумму 1000 руб. успешно зачислен.',         N'Info',    '20250108 11:31:00', 1, 2),
        (N'Ошибка соединения',    N'Зафиксированы перебои связи в вашем районе. Устраняем.',  N'Error',   '20250113 15:00:00', 0, 4),
        (N'Системное обновление', N'Оборудование успешно обновлено. Скорость увеличена.',     N'Info',    '20250115 06:00:00', 0, NULL);

    COMMIT TRANSACTION;
    PRINT 'Готово! По 5 записей в каждую таблицу успешно добавлено.';

END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT '=== ОШИБКА — транзакция отменена ===';
    PRINT 'Сообщение : ' + ERROR_MESSAGE();
    PRINT 'Процедура : ' + ISNULL(ERROR_PROCEDURE(), 'нет');
    PRINT 'Строка    : ' + CAST(ERROR_LINE()   AS NVARCHAR(10));
    PRINT 'Код       : ' + CAST(ERROR_NUMBER() AS NVARCHAR(10));
END CATCH;
GO
