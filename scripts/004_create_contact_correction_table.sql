-- ContactCorrection: an audit trail row for every correction made to a Contact.
-- References Contact and Manager, so 001, 002 and 003 must run first.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ContactCorrection')
BEGIN
    CREATE TABLE dbo.ContactCorrection
    (
        Id                    INT IDENTITY(1,1) NOT NULL,
        ContactId             INT                NOT NULL,
        PreviousContactDate   DATE               NOT NULL,
        PreviousChannel       NVARCHAR(20)       NOT NULL,
        PreviousResult        NVARCHAR(30)       NOT NULL,
        PreviousNotes         NVARCHAR(500)      NULL,
        CorrectedByManagerId  INT                NOT NULL,
        Reason                NVARCHAR(300)      NOT NULL,
        CorrectionDate        DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_ContactCorrection PRIMARY KEY (Id),

        CONSTRAINT FK_ContactCorrection_Contact
            FOREIGN KEY (ContactId) REFERENCES dbo.Contact (Id),

        CONSTRAINT FK_ContactCorrection_Manager
            FOREIGN KEY (CorrectedByManagerId) REFERENCES dbo.Manager (Id)
    );
END;
GO
