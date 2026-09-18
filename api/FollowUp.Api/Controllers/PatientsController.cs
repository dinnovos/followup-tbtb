using FollowUp.Api.Dtos;
using FollowUp.Application.Entities;
using FollowUp.Application.Repositories;
using FollowUp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FollowUp.Api.Controllers;

[ApiController]
[Route("api/patients")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IPatientRepository _patientRepository;

    public PatientsController(IPatientService patientService, IPatientRepository patientRepository)
    {
        _patientService = patientService;
        _patientRepository = patientRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Register(CreatePatientRequest request)
    {
        // request.Country/DocumentType/TreatmentStartDate are nullable so that
        // "omitted" and "explicitly the first enum value" are distinguishable
        // (see CreatePatientRequest.Validate). [ApiController] already ran that
        // validation before this method executes, so .Value is safe here.
        var patient = new Patient
        {
            RegisteredByManagerId = request.RegisteredByManagerId,
            Country = request.Country!.Value,
            DocumentType = request.DocumentType!.Value,
            DocumentNumber = request.DocumentNumber,
            Name = request.Name,
            Phone = request.Phone,
            Email = request.Email,
            City = request.City,
            TreatmentStartDate = request.TreatmentStartDate!.Value
        };

        var registered = await _patientService.RegisterAsync(patient);
        var response = PatientResponse.FromEntity(registered);

        return Created($"/api/patients/{response.Id}", response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientRepository.GetAllAsync();
        var response = patients.Select(PatientListItemResponse.FromEntity);
        return Ok(response);
    }
}
