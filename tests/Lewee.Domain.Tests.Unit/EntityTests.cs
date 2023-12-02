using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class EntityTests
{
    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_AnotherType_ReturnsFalse()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var integer = 5;

        person.Equals(integer).Should().BeFalse();
    }

    [Fact]
    public void Equals_ReferenceEquals_ReturnsTrue()
    {
        var person1 = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var person2 = person1;

        person1.Equals(person2).Should().BeTrue();
    }

    [Fact]
    public void Equals_SameId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var person1 = new Person(id, "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var person2 = new Person(id, "Jane", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person1.Equals(person2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var person1 = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var person2 = new Person(Guid.NewGuid(), "Jane", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person1.Equals(person2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SameId_ReturnsSameHashCode()
    {
        var id = Guid.NewGuid();
        var person1 = new Person(id, "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var person2 = new Person(id, "Jane", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person1.GetHashCode().Should().Be(person2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentId_ReturnsDifferentHashCode()
    {
        var person1 = new Person(Guid.NewGuid(), "Jane", "Doe", DateOnly.FromDateTime(DateTime.Now));
        var person2 = new Person(Guid.NewGuid(), "Jane", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person1.GetHashCode().Should().NotBe(person2.GetHashCode());
    }

    [Fact]
    public void Delete_ValidPerson_SetsIsDeletedToTrue()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person.Delete();

        person.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Delete_AlreadyDeletedPerson_KeepsIsDeletedTrue()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        person.Delete();

        person.Delete();

        person.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public void Undelete_DeletedPerson_SetsIsDeletedToFalse()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));
        person.Delete();

        person.Undelete();

        person.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public void Undelete_NotDeletedPerson_KeepsIsDeletedFalse()
    {
        var person = new Person(Guid.NewGuid(), "John", "Doe", DateOnly.FromDateTime(DateTime.Now));

        person.Undelete();

        person.IsDeleted.Should().BeFalse();
    }
}
