namespace FollowUp.Application.Exceptions;

public class ContactNotFoundException : Exception
{
    public ContactNotFoundException(int contactId)
        : base($"No contact exists with id {contactId}.")
    {
    }
}
