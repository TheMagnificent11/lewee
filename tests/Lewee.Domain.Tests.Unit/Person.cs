using Lewee.Domain;

public class Person : Entity
{
    public Person(string givenName, string surname, DateOnly dateOfBirth)
    {
        this.GivenName = givenName;
        this.Surname = surname;
        this.DateOfBirth = dateOfBirth;
    }

    public Person(Guid id, string givenName, string surname, DateOnly dateOfBirth)
        : base(id)
    {
        this.GivenName = givenName;
        this.Surname = surname;
        this.DateOfBirth = dateOfBirth;
    }

    public string GivenName { get; }
    public string Surname { get; }
    public DateOnly DateOfBirth { get; }
}
