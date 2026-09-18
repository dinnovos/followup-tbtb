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

    [Fact]
    public async Task CA3_CorrectContact_UpdatesContactAndRecordsCorrection()
    {
        var existingContact = new Contact
        {
            Id = 5,
            PatientId = 10,
            ManagerId = 1,
            ContactDate = new DateOnly(2026, 1, 15),
            Channel = Channel.Call,
            Result = ContactResult.NotAnswered,
            Notes = "Nota original"
        };

        var contactRepository = new Mock<IContactRepository>();
        contactRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existingContact);

        var patientRepository = new Mock<IPatientRepository>();
        var service = new ContactService(patientRepository.Object, contactRepository.Object);

        var corrected = await service.CorrectAsync(
            contactId: 5,
            correctedByManagerId: 2,
            reason: "Canal registrado por error",
            contactDate: null,
            channel: Channel.WhatsApp,
            result: null,
            notes: null);

        Assert.Same(existingContact, corrected);
        Assert.Equal(Channel.WhatsApp, corrected.Channel);
        Assert.Equal(ContactResult.NotAnswered, corrected.Result); // no se tocó
        Assert.Equal("Nota original", corrected.Notes); // no se tocó

        contactRepository.Verify(r => r.CorrectAsync(
            existingContact,
            It.Is<ContactCorrection>(c =>
                c.ContactId == 5 &&
                c.PreviousChannel == Channel.Call && // el valor de ANTES de corregir
                c.CorrectedByManagerId == 2 &&
                c.Reason == "Canal registrado por error")),
            Times.Once);
    }

    [Fact]
    public async Task CA3_CorrectContactWithNoFieldsProvided_ThrowsArgumentException()
    {
        var contactRepository = new Mock<IContactRepository>();
        var patientRepository = new Mock<IPatientRepository>();
        var service = new ContactService(patientRepository.Object, contactRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CorrectAsync(5, 2, "Motivo", null, null, null, null));

        // No debe ni siquiera consultar el contacto: la guarda corta antes de tocar el repositorio.
        contactRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CA3_CorrectNonexistentContact_ThrowsContactNotFoundException()
    {
        var contactRepository = new Mock<IContactRepository>();
        contactRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contact?)null);

        var patientRepository = new Mock<IPatientRepository>();
        var service = new ContactService(patientRepository.Object, contactRepository.Object);

        await Assert.ThrowsAsync<ContactNotFoundException>(() =>
            service.CorrectAsync(999, 2, "Motivo", null, Channel.WhatsApp, null, null));

        contactRepository.Verify(
            r => r.CorrectAsync(It.IsAny<Contact>(), It.IsAny<ContactCorrection>()),
            Times.Never);
    }
}
