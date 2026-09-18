-- Seed data for local development and grading. All names are fictional,
-- not real people -- consistent with working in a regulated, health-data
-- domain (see hallazgo #8 in 01-hallazgos.md).

IF NOT EXISTS (SELECT 1 FROM dbo.Manager)
BEGIN
    INSERT INTO dbo.Manager (Name) VALUES
        ('Ana Gomez'),
        ('Carlos Ruiz'),
        ('Maria Torres');
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Patient)
BEGIN
    INSERT INTO dbo.Patient
        (RegisteredByManagerId, Country, DocumentType, DocumentNumber, Name, Phone, Email, City, TreatmentStartDate)
    VALUES
        ((SELECT Id FROM dbo.Manager WHERE Name = 'Ana Gomez'),
            'Colombia', 'CC', '1000000001', 'Laura Martinez', '3001112233',
            'laura.martinez@example.com', 'Bogota', '2026-01-10'),
        ((SELECT Id FROM dbo.Manager WHERE Name = 'Carlos Ruiz'),
            'Peru', 'DNI', '20000002', 'Jorge Salinas', '3002223344',
            NULL, 'Lima', '2026-02-01'),
        ((SELECT Id FROM dbo.Manager WHERE Name = 'Maria Torres'),
            'Ecuador', 'CI', '30000003', 'Sofia Vera', '3003334455',
            'sofia.vera@example.com', 'Quito', '2026-02-15');
END;
GO
