-- Patient: a person enrolled in the follow-up program.
-- References Manager (who registered the patient), so 001 must run first.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Patient')
BEGIN
    CREATE TABLE dbo.Patient
    (
        Id                     INT IDENTITY(1,1) NOT NULL,
        RegisteredByManagerId  INT                NOT NULL,
        Country                NVARCHAR(20)       NOT NULL,
        DocumentType           NVARCHAR(20)       NOT NULL,
        DocumentNumber         NVARCHAR(20)       NOT NULL,
        Name                   NVARCHAR(150)      NOT NULL,
        Phone                  NVARCHAR(20)       NOT NULL,
        Email                  NVARCHAR(150)      NULL,
        City                   NVARCHAR(100)      NOT NULL,
        TreatmentStartDate     DATE               NOT NULL,
        CreatedAt              DATETIME2          NOT NULL DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_Patient PRIMARY KEY (Id),

        CONSTRAINT FK_Patient_Manager
            FOREIGN KEY (RegisteredByManagerId) REFERENCES dbo.Manager (Id),

        CONSTRAINT CHK_Patient_Country
            CHECK (Country IN ('Colombia', 'Peru', 'Ecuador')),

        CONSTRAINT CHK_Patient_DocumentType
            CHECK (DocumentType IN ('CC', 'DNI', 'CI', 'Passport')),

        -- Resolves hallazgo #3 (01-hallazgos.md): a patient is unique per country,
        -- because the same document number/type can belong to different people
        -- in different countries.
        CONSTRAINT UQ_Patient_Document
            UNIQUE (Country, DocumentType, DocumentNumber)
    );
END;
GO
