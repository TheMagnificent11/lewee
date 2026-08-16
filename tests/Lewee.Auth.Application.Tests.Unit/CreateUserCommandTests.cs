using FluentAssertions;
using Lewee.Auth.Domain;
using Lewee.Domain;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

public sealed class CreateUserCommandTests
{
    [Fact]
    public void Validator_Should_Reject_Empty_ExternalUserId()
    {
        var validator = new CreateUserCommand.Validator();

        var result = validator.Validate(new CreateUserCommand(string.Empty));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validator_Should_Accept_Valid_ExternalUserId()
    {
        var validator = new CreateUserCommand.Validator();

        var result = validator.Validate(new CreateUserCommand("external-id"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Handler_Should_Create_User_Without_Membership()
    {
        var repository = new Mock<IRepository<User>>();
        User createdUser = null;
        repository
            .Setup(item => item.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => createdUser = user);
        var handler = new CreateUserCommand.Handler(
            repository.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            NullLogger<CreateUserCommand.Handler>.Instance);

        await handler.Handle(new CreateUserCommand("external-id"), CancellationToken.None);

        createdUser.Should().NotBeNull();
        createdUser.ExternalId.Should().Be("external-id");
        createdUser.TenantMemberships.Should().BeEmpty();
        repository.Verify(item => item.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Not_Create_Duplicate_User()
    {
        var existingUser = User.Create("external-id", Guid.NewGuid());
        var repository = new Mock<IRepository<User>>();
        repository
            .Setup(item => item.QueryOneAsync(
                It.IsAny<QuerySpecification<User>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingUser);
        var handler = new CreateUserCommand.Handler(
            repository.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            NullLogger<CreateUserCommand.Handler>.Instance);

        await handler.Handle(new CreateUserCommand("external-id"), CancellationToken.None);

        repository.Verify(
            item => item.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        repository.Verify(
            item => item.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
