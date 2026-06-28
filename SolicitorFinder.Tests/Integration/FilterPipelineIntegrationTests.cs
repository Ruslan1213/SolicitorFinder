using FluentAssertions;
using SolicitorFinder.Application.Pipelines;
using SolicitorFinder.Application.Pipelines.Filters;
using System.Linq.Expressions;

namespace SolicitorFinder.Tests.Integration;

public sealed class FilterPipelineIntegrationTests
{
    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private sealed class TestFilter : Filter<TestModel>
    {
        public TestFilter(Expression<Func<TestModel, bool>>? expression) : base(expression) { }
    }

    [Fact]
    public void Build_WithMultipleFilters_CreatesCorrectExpression()
    {
        // Arrange
        var items = new List<TestModel>
        {
            new TestModel { Name = "John", Age = 30 },
            new TestModel { Name = "Jane", Age = 25 },
            new TestModel { Name = "Bob", Age = 35 }
        }.AsQueryable();

        var pipeline = new FilterPipeline<TestModel>();
        pipeline.AddFilter(new TestFilter(m => m.Age >= 30));
        pipeline.AddFilter(new TestFilter(m => m.Name.StartsWith("J")));

        // Act
        var expression = pipeline.Build();
        var result = expression != null ? items.Where(expression.Compile()).ToList() : items.ToList();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("John");
    }

    [Fact]
    public void AddFilterIf_WhenConditionTrue_AddsFilter()
    {
        // Arrange
        var items = new List<TestModel>
        {
            new TestModel { Name = "John", Age = 30 },
            new TestModel { Name = "Jane", Age = 25 }
        }.AsQueryable();

        var pipeline = new FilterPipeline<TestModel>();
        pipeline.AddFilterIf(true, new TestFilter(m => m.Age >= 30));

        // Act
        var expression = pipeline.Build();
        var result = expression != null ? items.Where(expression.Compile()).ToList() : items.ToList();

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void AddFilterIf_WhenConditionFalse_DoesNotAddFilter()
    {
        // Arrange
        var items = new List<TestModel>
        {
            new TestModel { Name = "John", Age = 30 },
            new TestModel { Name = "Jane", Age = 25 }
        }.AsQueryable();

        var pipeline = new FilterPipeline<TestModel>();
        pipeline.AddFilterIf(false, new TestFilter(m => m.Age >= 30));

        // Act
        var expression = pipeline.Build();

        // Assert
        expression.Should().BeNull();
    }

    [Fact]
    public void Build_WithNoFilters_ReturnsNull()
    {
        // Arrange
        var pipeline = new FilterPipeline<TestModel>();

        // Act
        var expression = pipeline.Build();

        // Assert
        expression.Should().BeNull();
    }

    [Fact]
    public void AddFilter_ReturnsFilterPipeline_ForChaining()
    {
        // Arrange
        var pipeline = new FilterPipeline<TestModel>();

        // Act
        var result = pipeline
            .AddFilter(new TestFilter(m => m.Age >= 30))
            .AddFilter(new TestFilter(m => m.Name.Length > 0));

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<FilterPipeline<TestModel>>();
    }
}
