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
}
