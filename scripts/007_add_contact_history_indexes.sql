-- Indexes for the "consulta con criterio" (ContactRepository.GetHistoryByPatientIdAsync):
-- a foreign key alone does NOT create an index in SQL Server, it only enforces
-- referential integrity. Without these, the query's WHERE Contact.PatientId = @id
-- and its correlated subquery WHERE ContactCorrection.ContactId = @contactId
-- (run once per contact returned) both fall back to a full table scan, which
-- gets slower as every patient's contacts pile up in the same two tables --
-- not just the one patient being queried.

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Contact_PatientId' AND object_id = OBJECT_ID('dbo.Contact')
)
BEGIN
    CREATE INDEX IX_Contact_PatientId ON dbo.Contact (PatientId);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ContactCorrection_ContactId' AND object_id = OBJECT_ID('dbo.ContactCorrection')
)
BEGIN
    CREATE INDEX IX_ContactCorrection_ContactId ON dbo.ContactCorrection (ContactId);
END;
GO
