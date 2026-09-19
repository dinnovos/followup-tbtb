-- Seed contacts for the 3 patients from 005_seed_data.sql, so the history
-- screen (CA-3) has something to show without manual setup. Patients and
-- managers are looked up by their unique natural keys (DocumentNumber, Name)
-- instead of hardcoded ids, since both are IDENTITY columns whose actual
-- values depend on insertion order in whatever database this runs against.

IF NOT EXISTS (SELECT 1 FROM dbo.Contact)
BEGIN
    INSERT INTO dbo.Contact
        (PatientId, ManagerId, ContactDate, Channel, Result, Notes)
    VALUES
        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '1000000001'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Ana Gomez'),
            '2026-01-20', 'Call', 'NotAnswered', NULL),
        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '1000000001'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Ana Gomez'),
            '2026-02-05', 'WhatsApp', 'Answered', 'Paciente reporta buena adherencia al tratamiento'),

        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '20000002'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Carlos Ruiz'),
            '2026-02-10', 'Call', 'Answered', NULL),
        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '20000002'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Carlos Ruiz'),
            '2026-03-01', 'Email', 'DeclinedFollowUp', 'Solicita no ser contactado por ahora'),

        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '30000003'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Maria Torres'),
            '2026-02-25', 'WhatsApp', 'Answered', NULL),
        ((SELECT Id FROM dbo.Patient WHERE DocumentNumber = '30000003'),
            (SELECT Id FROM dbo.Manager WHERE Name = 'Maria Torres'),
            '2026-03-10', 'Call', 'NotAnswered', NULL);
END;
GO
