using Saji.Domain;
using Sample.Customers.Domain.Events;

namespace Sample.Customers.Domain.Entities;

public class Customer : BaseEntity
{
    /* TODO: Guard against empty strings */

    public Customer(
        string emailAddress,
        string givenName,
        string surname,
        Guid correlationId)
        : base()
    {
        this.EmailAddress = emailAddress.Trim().ToLower();
        this.GivenName = givenName.Trim();
        this.Surname = surname.Trim();

        if (correlationId == Guid.Empty)
        {
            correlationId = Guid.NewGuid();
        }

        this.DomainEvents.Raise(new CustomerCreatedEvent(correlationId, this.Id));
    }

    // EF constructor
    private Customer()
        : base()
    {
        this.EmailAddress = string.Empty;
        this.GivenName = string.Empty;
        this.Surname = string.Empty;
    }

    public string EmailAddress { get; protected set; }

    public string GivenName { get; protected set; }

    public string Surname { get; protected set; }

    public void ChangeName(string givenName, string surname, Guid correlationId)
    {
        var changed = false;

        givenName = givenName.Trim();
        surname = surname.Trim();

        if (this.GivenName != givenName)
        {
            this.GivenName = givenName;
            changed = true;
        }

        if (this.Surname != surname)
        {
            this.Surname = surname;
            changed = true;
        }

        if (!changed)
        {
            return;
        }

        this.ApplyTrackingData();

        if (correlationId == Guid.Empty)
        {
            correlationId = Guid.NewGuid();
        }

        this.DomainEvents.Raise(new CustomerUpdatedEvent(correlationId, this.Id));
    }

    public void ChangeEmailAddress(string emailAddress, Guid correlationId)
    {
        emailAddress = emailAddress.Trim().ToLower();

        if (this.EmailAddress == emailAddress)
        {
            return;
        }

        if (correlationId == Guid.Empty)
        {
            correlationId = Guid.NewGuid();
        }

        this.EmailAddress = emailAddress;
        this.ApplyTrackingData();
        this.DomainEvents.Raise(new CustomerUpdatedEvent(correlationId, this.Id));
    }

    public static class MaxFieldLengths
    {
        public const int EmailAddress = 255;

        public const int GivenName = 100;

        public const int Surname = 100;
    }
}
