using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace Lewee.Domain.Tests.Unit;

public class QuerySpecificationTests
{
    [Fact]
    public void QuerySpecification_WithWhere_ShouldStoreWhereExpression()
    {
        // Arrange & Act
        var spec = new TestQuerySpecification(x => x.Id == Guid.NewGuid());

        // Assert
        spec.WhereExpressions.Should().HaveCount(1);
        spec.WhereExpressions[0].Should().NotBeNull();
    }

    [Fact]
    public void QuerySpecification_WithMultipleWhere_ShouldStoreAllWhereExpressions()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var spec = new TestQuerySpecificationWithMultipleWhere(id);

        // Assert
        spec.WhereExpressions.Should().HaveCount(2);
    }

    [Fact]
    public void QuerySpecification_WithInclude_ShouldStoreIncludeExpression()
    {
        // Arrange & Act
        var spec = new TestQuerySpecificationWithInclude();

        // Assert
        spec.IncludeExpressions.Should().HaveCount(1);
        spec.IncludeExpressions[0].IsThenInclude.Should().BeFalse();
        spec.IncludeExpressions[0].Expression.Should().NotBeNull();
    }

    [Fact]
    public void QuerySpecification_WithIncludeAndThenInclude_ShouldStoreBothExpressions()
    {
        // Arrange & Act
        var spec = new TestQuerySpecificationWithIncludeAndThenInclude();

        // Assert
        spec.IncludeExpressions.Should().HaveCount(2);
        spec.IncludeExpressions[0].IsThenInclude.Should().BeFalse();
        spec.IncludeExpressions[1].IsThenInclude.Should().BeTrue();
    }

    [Fact]
    public void QuerySpecification_WithWhereAndInclude_ShouldStoreBoth()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var spec = new TestQuerySpecificationWithWhereAndInclude(id);

        // Assert
        spec.WhereExpressions.Should().HaveCount(1);
        spec.IncludeExpressions.Should().HaveCount(1);
    }

    [Fact]
    public void QuerySpecification_FluentChaining_ShouldWork()
    {
        // Arrange & Act
        var spec = new TestQuerySpecificationWithFluentChaining();

        // Assert
        spec.WhereExpressions.Should().HaveCount(2);
        spec.IncludeExpressions.Should().HaveCount(2);
    }

    // Test helper classes
    private sealed class TestAggregate : AggregateRoot
    {
        public string Name { get; set; } = string.Empty;
    }

    private sealed class TestRelatedAggregate : AggregateRoot
    {
        public string Description { get; set; } = string.Empty;
    }

    private sealed class TestQuerySpecification : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecification(Expression<Func<TestAggregate, bool>> predicate)
        {
            this.Query.Where(predicate);
        }
    }

    private sealed class TestQuerySpecificationWithMultipleWhere : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecificationWithMultipleWhere(Guid id)
        {
            this.Query
                .Where(x => x.Id == id)
                .Where(x => x.Name != null);
        }
    }

    private sealed class TestQuerySpecificationWithInclude : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecificationWithInclude()
        {
            this.Query.Include(x => x.Name);
        }
    }

    private sealed class TestQuerySpecificationWithIncludeAndThenInclude : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecificationWithIncludeAndThenInclude()
        {
            this.Query
                .Include(x => x.Name)
                .ThenInclude<TestRelatedAggregate, string>(x => x.Description);
        }
    }

    private sealed class TestQuerySpecificationWithWhereAndInclude : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecificationWithWhereAndInclude(Guid id)
        {
            this.Query
                .Where(x => x.Id == id)
                .Include(x => x.Name);
        }
    }

    private sealed class TestQuerySpecificationWithFluentChaining : QuerySpecification<TestAggregate>
    {
        public TestQuerySpecificationWithFluentChaining()
        {
            this.Query
                .Where(x => x.Id != Guid.Empty)
                .Include(x => x.Name)
                .ThenInclude<TestRelatedAggregate, string>(x => x.Description)
                .Where(x => x.Name != null);
        }
    }
}
