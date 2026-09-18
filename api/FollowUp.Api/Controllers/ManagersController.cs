using FollowUp.Api.Dtos;
using FollowUp.Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FollowUp.Api.Controllers;

[ApiController]
[Route("api/managers")]
public class ManagersController : ControllerBase
{
    private readonly IManagerRepository _managerRepository;

    public ManagersController(IManagerRepository managerRepository)
    {
        _managerRepository = managerRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var managers = await _managerRepository.GetAllAsync();
        var response = managers.Select(ManagerResponse.FromEntity);
        return Ok(response);
    }
}
