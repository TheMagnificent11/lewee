using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_SetsMessage()
    {
        // Arrange
        var message = "This is a domain exception";

        // Act
        var exception = new DomainException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void DomainException_CanBeThrown()
    {
        // Arrange
        var message = "Domain rule violated";

        // Act
        Action act = () => throw new DomainException(message);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage(message);
    }

    [Fact]
    public void DomainException_CanBeCaught()
    {
        // Arrange
        var message = "Domain rule violated";
        DomainException caughtException = null;

        // Act
        try
        {
            throw new DomainException(message);
        }
        catch (DomainException ex)
        {
            caughtException = ex;
        }

        // Assert
        caughtException.Should().NotBeNull();
        caughtException.Message.Should().Be(message);
    }

    [Fact]
    public void DomainException_InheritsFromException()
    {
        // Arrange
        var message = "Domain exception";

        // Act
        var exception = new DomainException(message);

        // Assert
        exception.Should().BeAssignableTo<Exception>();
    }
}
