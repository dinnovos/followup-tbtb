using FollowUp.Api.Dtos;
using FollowUp.Application.Entities;
using FollowUp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FollowUp.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId}/contacts")]
public class ContactsController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetHistoryByPatientId(int patientId)
    {
        var history = await _contactService.GetHistoryByPatientIdAsync(patientId);
        var response = history.Select(ContactWithHistoryResponse.FromEntity);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Register(int patientId, CreateContactRequest request)
    {
        // See CreateContactRequest.Validate: ContactDate/Channel/Result are nullable
        // so "omitted" is distinguishable from "explicitly the first enum value".
        // [ApiController] already validated this before the method runs.
        var contact = new Contact
        {
            ManagerId = request.ManagerId,
            ContactDate = request.ContactDate!.Value,
            Channel = request.Channel!.Value,
            Result = request.Result!.Value,
            Notes = request.Notes
        };

        var registered = await _contactService.RegisterAsync(patientId, contact);
        var response = ContactResponse.FromEntity(registered);

        return Created($"/api/patients/{patientId}/contacts/{response.Id}", response);
    }

    // Absolute route ("/api/contacts/{id}"): this action does not live under
    // /api/patients/{patientId}/... like Register does above -- the contract
    // (02-plan.md) puts correction at the contact's own root, not nested.
    [HttpPut("/api/contacts/{id}")]
    public async Task<IActionResult> Correct(int id, CorrectContactRequest request)
    {
        var corrected = await _contactService.CorrectAsync(
            id,
            request.CorrectedByManagerId,
            request.Reason,
            request.ContactDate,
            request.Channel,
            request.Result,
            request.Notes);

        var response = ContactResponse.FromEntity(corrected);
        return Ok(response);
    }
}
