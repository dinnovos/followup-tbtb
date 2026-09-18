namespace FollowUp.Application.Exceptions;

public class PatientNotFoundException : Exception
{
    public PatientNotFoundException(int patientId)
        : base($"No patient exists with id {patientId}.")
    {
    }
}
