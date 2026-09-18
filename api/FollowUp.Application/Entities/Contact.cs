namespace FollowUp.Application.Entities;

public class Contact
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int ManagerId { get; set; }
    public DateOnly ContactDate { get; set; }
    public Channel Channel { get; set; }
    public ContactResult Result { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
