using FluentAssertions;
using SolicitorFinder.GeneralParser.Core;
using SolicitorFinder.GeneralParser.Enums;

namespace SolicitorFinder.Tests.Unit.Parser;

public sealed class HtmlParserTests
{
    private readonly HtmlParser _sut;

    public HtmlParserTests()
    {
        _sut = new HtmlParser();
    }

    [Fact]
    public void ParseElements_WithValidHtml_ReturnsElements()
    {
        // Arrange
        var html = "<div class='test'><p>Hello World</p></div>";
        var selector = "div.test";

        // Act
        var result = _sut.ParseElements(html, selector, SearchType.All);

        // Assert
        result.Should().HaveCount(1);
        result.First().TagName.Should().Be("div");
        result.First().ClassName.Should().Be("test");
    }

    [Fact]
    public void ParseElements_WithMultipleElements_ReturnsAll()
    {
        // Arrange
        var html = "<div class='item'>First</div><div class='item'>Second</div><div class='item'>Third</div>";
        var selector = "div.item";

        // Act
        var result = _sut.ParseElements(html, selector, SearchType.All);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public void ExtractText_WithValidSelector_ReturnsText()
    {
        // Arrange
        var html = "<div><p id='content'>Hello World</p></div>";
        var selector = "p#content";

        // Act
        var result = _sut.ExtractText(html, selector);

        // Assert
        result.Should().Be("Hello World");
    }

    [Fact]
    public void ExtractText_WithInvalidSelector_ReturnsNull()
    {
        // Arrange
        var html = "<div><p>Hello</p></div>";
        var selector = "span#missing";

        // Act
        var result = _sut.ExtractText(html, selector);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractAttribute_WithValidSelector_ReturnsAttribute()
    {
        // Arrange
        var html = "<a href='https://example.com' class='link'>Link</a>";
        var selector = "a.link";
        var attribute = "href";

        // Act
        var result = _sut.ExtractAttribute(html, selector, attribute);

        // Assert
        result.Should().Be("https://example.com");
    }

    [Fact]
    public void ParseElements_WithNestedElements_ParsesChildren()
    {
        // Arrange
        var html = "<ul><li>Item 1</li><li>Item 2</li></ul>";
        var selector = "ul";

        // Act
        var result = _sut.ParseElements(html, selector, SearchType.First);

        // Assert
        result.Should().HaveCount(1);
        result.First().Children.Should().HaveCount(2);
        result.First().Children.First().InnerText.Should().Be("Item 1");
    }
}
