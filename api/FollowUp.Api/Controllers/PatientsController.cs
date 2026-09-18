using FollowUp.Api.Dtos;
using FollowUp.Application.Entities;
using FollowUp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FollowUp.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;

    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    public async Task<IActionResult> Register(CreatePatientRequest request)
    {
        var patient = new Patient
        {
            RegisteredByManagerId = request.RegisteredByManagerId,
            Country = request.Country,
            DocumentType = request.DocumentType,
            DocumentNumber = request.DocumentNumber,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            City = request.City,
            TreatmentStartDate = request.TreatmentStartDate
        };

        var registered = await _patientService.RegisterAsync(patient);
        var response = PatientResponse.FromEntity(registered);

        return Created($"/api/patients/{response.Id}", response);
    }
}
