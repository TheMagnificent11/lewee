using GuardNet;
using Microsoft.AspNetCore.Identity;
using Saji.Domain;
using Sample.Customers.Contracts.Events;

namespace Sample.Customers.Domain.Entities;

public class Customer : BaseEntity
{
    public Customer(
        IdentityUser<string> user,
        string givenName,
        string surname,
        Guid correlationId)
        : base()
    {
        Guard.NotNullOrWhitespace(givenName, nameof(givenName), "Given Name is required");
        Guard.NotNullOrWhitespace(surname, nameof(surname), "Surname is required");

        this.UserId = user.Id;
        this.GivenName = givenName.Trim();
        this.Surname = surname.Trim();

        if (correlationId == Guid.Empty)
        {
            correlationId = Guid.NewGuid();
        }

        this.DomainEvents.Raise(new CustomerCreatedEvent(
            correlationId,
            this.Id,
            this.GivenName,
            this.Surname));
    }

    // EF constructor
    private Customer()
        : base()
    {
        this.UserId = string.Empty;
        this.GivenName = string.Empty;
        this.Surname = string.Empty;
    }

    public string UserId { get; protected set; }

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

        this.DomainEvents.Raise(new CustomerUpdatedEvent(
            correlationId,
            this.Id,
            this.GivenName,
            this.Surname));
    }

    public static class MaxFieldLengths
    {
        public const int UserId = 100;

        public const int GivenName = 100;

        public const int Surname = 100;
    }
}
