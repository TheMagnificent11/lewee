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
    public void Should_RejectCommand_When_ExternalUserIdEmpty()
    {
        var validator = new CreateUserCommand.Validator();

        var result = validator.Validate(new CreateUserCommand(string.Empty));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Should_AcceptCommand_When_ExternalUserIdValid()
    {
        const string externalUserId = "external-id";
        var validator = new CreateUserCommand.Validator();

        var result = validator.Validate(new CreateUserCommand(externalUserId));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CreateUserWithoutMembership_When_UserDoesNotExist()
    {
        const string externalUserId = "external-id";
        var repository = new Mock<IRepository<User>>();
        User createdUser = null;
        repository
            .Setup(item => item.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => createdUser = user);
        var handler = new CreateUserCommand.Handler(
            repository.Object,
            Mock.Of<Correlate.ICorrelationContextAccessor>(),
            NullLogger<CreateUserCommand.Handler>.Instance);

        await handler.Handle(new CreateUserCommand(externalUserId), CancellationToken.None);

        createdUser.Should().NotBeNull();
        createdUser.ExternalId.Should().Be(externalUserId);
        createdUser.TenantMemberships.Should().BeEmpty();
        repository.Verify(item => item.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Should_NotCreateDuplicateUser_When_UserAlreadyExists()
    {
        const string externalUserId = "external-id";
        var existingUser = User.Create(externalUserId, Guid.NewGuid());
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

        await handler.Handle(new CreateUserCommand(externalUserId), CancellationToken.None);

        repository.Verify(
            item => item.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
        repository.Verify(
            item => item.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
