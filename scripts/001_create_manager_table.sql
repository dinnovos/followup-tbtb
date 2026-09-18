-- Manager: catalog of case managers ("gestores").
-- No FKs in or out yet, so it must be the first table created --
-- Patient, Contact, and ContactCorrection all reference it later.
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Manager')
BEGIN
    CREATE TABLE dbo.Manager
    (
        Id   INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(150)     NOT NULL,
        CONSTRAINT PK_Manager PRIMARY KEY (Id)
    );
END;
GO
