using FollowUp.Application.Entities;

namespace FollowUp.Api.Dtos;

public class ManagerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static ManagerResponse FromEntity(Manager manager) => new()
    {
        Id = manager.Id,
        Name = manager.Name
    };
}
