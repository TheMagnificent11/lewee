using FluentAssertions;
using FluentValidation.Results;
using Lewee.Application.Mediation.Requests;
using Xunit;

namespace Lewee.Application.Tests.Unit;

/// <summary>
/// Tests for Result classes (CommandResult, QueryResult)
/// </summary>
public class ResultTests
{
    [Fact]
    public void CommandResult_Success_ShouldCreateSuccessResult()
    {
        // Act
        var result = CommandResult.Success();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Success);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void CommandResult_FailWithMessage_ShouldCreateFailedResult()
    {
        // Arrange
        var message = "Operation failed";

        // Act
        var result = CommandResult.Fail(ResultStatus.BadRequest, message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be(message);
    }

    [Fact]
    public void CommandResult_FailWithStatusAndMessage_ShouldCreateFailedResultWithStatus()
    {
        // Arrange
        var status = ResultStatus.NotFound;
        var message = "Resource not found";

        // Act
        var result = CommandResult.Fail(status, message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(status);
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be(message);
    }

    [Fact]
    public void CommandResult_FailWithValidationFailures_ShouldCreateFailedResultWithErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Email", "Invalid email format")
        };

        // Act
        var result = CommandResult.Fail(ResultStatus.BadRequest, failures);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Errors.Should().ContainKey("Name");
        result.Errors.Should().ContainKey("Email");
        result.Errors["Name"].Should().Contain("Name is required");
        result.Errors["Email"].Should().Contain("Invalid email format");
    }

    [Fact]
    public void QueryResult_Success_ShouldCreateSuccessResultWithData()
    {
        // Arrange
        var data = new TestData("Test Value");

        // Act
        var result = QueryResult<TestData>.Success(data);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Success);
        result.Data.Should().Be(data);
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void QueryResult_Fail_ShouldCreateFailedResult()
    {
        // Arrange
        var message = "Query failed";

        // Act
        var result = QueryResult<TestData>.Fail(ResultStatus.BadRequest, message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(ResultStatus.BadRequest);
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be(message);
    }

    [Fact]
    public void QueryResult_FailWithStatus_ShouldCreateFailedResultWithStatus()
    {
        // Arrange
        var status = ResultStatus.NotFound;
        var message = "Data not found";

        // Act
        var result = QueryResult<TestData>.Fail(status, message);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Status.Should().Be(status);
        result.Data.Should().BeNull();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be(message);
    }

    [Theory]
    [InlineData(ResultStatus.Success)]
    [InlineData(ResultStatus.BadRequest)]
    [InlineData(ResultStatus.NotFound)]
    [InlineData(ResultStatus.Unauthenticated)]
    [InlineData(ResultStatus.Unauthorized)]
    public void ResultStatus_ShouldHaveExpectedValues(ResultStatus status)
    {
        // Act & Assert
        Enum.IsDefined(typeof(ResultStatus), status).Should().BeTrue();
    }

    [Fact]
    public void CommandResult_MultipleValidationFailuresForSameProperty_ShouldGroupErrors()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Name", "Name must be at least 3 characters long"),
            new("Email", "Invalid email format")
        };

        // Act
        var result = CommandResult.Fail(ResultStatus.BadRequest, failures);

        // Assert
        result.Errors.Should().HaveCount(3);
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name must be at least 3 characters long");
        result.Errors.Should().Contain(e => e.PropertyName == "Email" && e.ErrorMessage == "Invalid email format");
    }

    [Fact]
    public void Result_GenerateErrorMessage_ShouldCreateFormattedErrorMessage()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Name", "Name is required"),
            new("Email", "Invalid email format")
        };
        var result = CommandResult.Fail(ResultStatus.BadRequest, failures);

        // Act
        var errorMessage = result.GenerateErrorMessage();

        // Assert
        errorMessage.Should().Contain("Name: Name is required");
        errorMessage.Should().Contain("Email: Invalid email format");
    }
}