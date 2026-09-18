using FollowUp.Application.Entities;

namespace FollowUp.Application.Exceptions;

public class DuplicatePatientException : Exception
{
    public DuplicatePatientException(Country country, DocumentType documentType, string documentNumber)
        : base($"A patient with document {documentType} {documentNumber} already exists in {country}.")
    {
    }
}
