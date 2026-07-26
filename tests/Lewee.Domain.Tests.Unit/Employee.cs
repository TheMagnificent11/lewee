namespace Lewee.Domain.Tests.Unit;

internal sealed class Employee : Entity
{
    public Employee(Guid id, string givenName, string surname, DateOnly dateOfBirth, string employeeNumber)
        : base(id)
    {
        this.GivenName = givenName;
        this.Surname = surname;
        this.DateOfBirth = dateOfBirth;
        this.EmployeeNumber = employeeNumber;
    }

    public string GivenName { get; }

    public string Surname { get; }

    public DateOnly DateOfBirth { get; }

    public string EmployeeNumber { get; }
}
