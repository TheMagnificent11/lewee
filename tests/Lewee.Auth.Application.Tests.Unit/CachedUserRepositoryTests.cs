using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Lewee.Auth.Domain;
using Lewee.Domain;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Lewee.Auth.Application.Tests.Unit;

[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Test context handles disposal")]
public sealed class CachedUserRepositoryTests
{
    [Fact]
    public async Task Should_OnlyQueryInnerRepositoryOnce_When_RetrievingTheSameUserByIdAsync()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var innerRepository = new Mock<IRepository<User>>();
        innerRepository
            .Setup(item => item.RetrieveByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var repository = CreateRepository(innerRepository.Object);

        var first = await repository.RetrieveByIdAsync(user.Id);
        var second = await repository.RetrieveByIdAsync(user.Id);

        first.Should().BeSameAs(user);
        second.Should().BeSameAs(user);
        innerRepository.Verify(item => item.RetrieveByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_OnlyQueryInnerRepositoryOnce_When_QueryingTheSameExternalIdAsync()
    {
        const string externalId = "external-id";
        var user = User.Create(externalId, Guid.NewGuid());
        var innerRepository = new Mock<IRepository<User>>();
        innerRepository
            .Setup(item => item.QueryOneAsync(
                It.Is<UserByExternalIdSpecification>(spec => spec.ExternalId == externalId),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var repository = CreateRepository(innerRepository.Object);

        var first = await repository.QueryOneAsync(new UserByExternalIdSpecification(externalId));
        var second = await repository.QueryOneAsync(new UserByExternalIdSpecification(externalId));

        first.Should().BeSameAs(user);
        second.Should().BeSameAs(user);
        innerRepository.Verify(
            item => item.QueryOneAsync(
                It.Is<UserByExternalIdSpecification>(spec => spec.ExternalId == externalId),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Should_AlwaysDelegateToInnerRepository_When_QuerySpecificationIsNotByExternalIdAsync()
    {
        var user = User.Create("external-id", Guid.NewGuid());
        var specification = new TestUserSpecification(user.Id);
        var innerRepository = new Mock<IRepository<User>>();
        innerRepository
            .Setup(item => item.QueryOneAsync(specification, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var repository = CreateRepository(innerRepository.Object);

        await repository.QueryOneAsync(specification);
        await repository.QueryOneAsync(specification);

        innerRepository.Verify(
            item => item.QueryOneAsync(specification, It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task Should_QueryInnerRepositoryAgain_When_CacheEntryExpiresAsync()
    {
        var originalDuration = CachedUserRepository.CacheDuration;
        CachedUserRepository.CacheDuration = TimeSpan.FromMilliseconds(1);

        try
        {
            var user = User.Create("external-id", Guid.NewGuid());
            var innerRepository = new Mock<IRepository<User>>();
            innerRepository
                .Setup(item => item.RetrieveByIdAsync(user.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var repository = CreateRepository(innerRepository.Object);

            await repository.RetrieveByIdAsync(user.Id);
            await Task.Delay(20);
            await repository.RetrieveByIdAsync(user.Id);

            innerRepository.Verify(
                item => item.RetrieveByIdAsync(user.Id, It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }
        finally
        {
            CachedUserRepository.CacheDuration = originalDuration;
        }
    }

    private static CachedUserRepository CreateRepository(IRepository<User> innerRepository) =>
        new(innerRepository, new MemoryCache(new MemoryCacheOptions()));

    private sealed class TestUserSpecification : QuerySpecification<User>
    {
        public TestUserSpecification(Guid userId)
        {
            this.Query.Where(user => user.Id == userId);
        }
    }
}
