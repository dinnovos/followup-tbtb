using FollowUp.Application.Entities;
using FollowUp.Application.Exceptions;
using FollowUp.Application.Repositories;
using FollowUp.Application.Services;
using Moq;

namespace FollowUp.Application.Tests;

public class PatientServiceTests
{
    private static Patient BuildPatient() => new()
    {
        RegisteredByManagerId = 1,
        Country = Country.Colombia,
        DocumentType = DocumentType.CC,
        DocumentNumber = "123456",
        Name = "Ana Gomez",
        Phone = "3000000000",
        City = "Bogota",
        TreatmentStartDate = new DateOnly(2026, 1, 1)
    };

    [Fact]
    public async Task CA1_RegisterNewPatient_IsAddedSuccessfully()
    {
        var patient = BuildPatient();
        var repository = new Mock<IPatientRepository>();
        repository
            .Setup(r => r.ExistsAsync(patient.Country, patient.DocumentType, patient.DocumentNumber))
            .ReturnsAsync(false);

        var service = new PatientService(repository.Object);

        var result = await service.RegisterAsync(patient);

        Assert.Same(patient, result);
        repository.Verify(r => r.AddAsync(patient), Times.Once);
    }

    [Fact]
    public async Task CA1_RegisterDuplicatePatient_ThrowsDuplicatePatientException()
    {
        var patient = BuildPatient();
        var repository = new Mock<IPatientRepository>();
        repository
            .Setup(r => r.ExistsAsync(patient.Country, patient.DocumentType, patient.DocumentNumber))
            .ReturnsAsync(true);

        var service = new PatientService(repository.Object);

        await Assert.ThrowsAsync<DuplicatePatientException>(() => service.RegisterAsync(patient));
        repository.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Never);
    }
}
