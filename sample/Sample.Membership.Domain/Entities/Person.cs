using Saji.Domain;
using Sample.Membership.Domain.ValueObjects;

namespace Sample.Membership.Domain.Entities;

public class Person : BaseEntity
{
    public Person(string givenName, string surname)
    {
        this.GivenName = givenName;
        this.Surname = surname;
    }

    public Person(string givenName, string surname, DateOnly dateOfBirth)
        : this(givenName, surname)
    {
        this.DateOfBirth = dateOfBirth;
    }

    public string GivenName { get; }
    public string Surname { get; }
    public DateOnly? DateOfBirth { get; private set; }
    public string? EmailAddress { get; private set; }
    public Address? Address { get; private set; }
    public Address? PostalAddress { get; private set; }

    public void SetEmailAddress(string emailAddress)
    {
        if (emailAddress.Equals(this.EmailAddress, StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        this.EmailAddress = emailAddress.ToLowerInvariant();
    }

    public void SetAddress(Address address)
    {
        throw new NotImplementedException();
    }

    public void SetPostalAddress(Address address)
    {
        throw new NotImplementedException();
    }
}
