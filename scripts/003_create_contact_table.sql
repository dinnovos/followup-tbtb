-- Contact: a single contact attempt a manager makes with a patient.
-- References Patient and Manager, so 001 and 002 must run first.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Contact')
BEGIN
    CREATE TABLE dbo.Contact
    (
        Id          INT IDENTITY(1,1) NOT NULL,
        PatientId   INT                NOT NULL,
        ManagerId   INT                NOT NULL,
        ContactDate DATE               NOT NULL,
        Channel     NVARCHAR(20)       NOT NULL,
        Result      NVARCHAR(30)       NOT NULL,
        Notes       NVARCHAR(500)      NULL,
        CreatedAt   DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAt   DATETIME2          NULL,

        CONSTRAINT PK_Contact PRIMARY KEY (Id),

        CONSTRAINT FK_Contact_Patient
            FOREIGN KEY (PatientId) REFERENCES dbo.Patient (Id),

        CONSTRAINT FK_Contact_Manager
            FOREIGN KEY (ManagerId) REFERENCES dbo.Manager (Id),

        CONSTRAINT CHK_Contact_Channel
            CHECK (Channel IN ('Call', 'WhatsApp', 'Email')),

        -- Resolves hallazgo #6 (01-hallazgos.md): the PRD never defines a closed
        -- catalog of contact results, so we fix one here at the database level.
        CONSTRAINT CHK_Contact_Result
            CHECK (Result IN ('Answered', 'NotAnswered', 'DeclinedFollowUp'))
    );
END;
GO
