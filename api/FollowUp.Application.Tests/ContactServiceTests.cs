using FollowUp.Application.Entities;
using FollowUp.Application.Exceptions;
using FollowUp.Application.Repositories;
using FollowUp.Application.Services;
using Moq;

namespace FollowUp.Application.Tests;

public class ContactServiceTests
{
    private static Contact BuildContact() => new()
    {
        ManagerId = 1,
        ContactDate = new DateOnly(2026, 1, 15),
        Channel = Channel.Call,
        Result = ContactResult.Answered
    };

    [Fact]
    public async Task CA2_RegisterContact_IsAddedSuccessfully()
    {
        const int patientId = 10;
        var contact = BuildContact();

        var patientRepository = new Mock<IPatientRepository>();
        patientRepository.Setup(r => r.ExistsByIdAsync(patientId)).ReturnsAsync(true);

        var contactRepository = new Mock<IContactRepository>();

        var service = new ContactService(patientRepository.Object, contactRepository.Object);

        var result = await service.RegisterAsync(patientId, contact);

        Assert.Same(contact, result);
        Assert.Equal(patientId, result.PatientId);
        contactRepository.Verify(r => r.AddAsync(contact), Times.Once);
    }

    [Fact]
    public async Task CA2_RegisterContactForNonexistentPatient_ThrowsPatientNotFoundException()
    {
        const int patientId = 999;
        var contact = BuildContact();

        var patientRepository = new Mock<IPatientRepository>();
        patientRepository.Setup(r => r.ExistsByIdAsync(patientId)).ReturnsAsync(false);

        var contactRepository = new Mock<IContactRepository>();

        var service = new ContactService(patientRepository.Object, contactRepository.Object);

        await Assert.ThrowsAsync<PatientNotFoundException>(() => service.RegisterAsync(patientId, contact));
        contactRepository.Verify(r => r.AddAsync(It.IsAny<Contact>()), Times.Never);
    }
}
