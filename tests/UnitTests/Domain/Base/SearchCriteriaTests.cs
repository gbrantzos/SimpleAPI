using FluentAssertions;
using SimpleAPI.Domain.Base;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Domain.Base;

public class SearchCriteriaTests
{
    [Fact]
    public void Should_parse_includes()
    {
        // Arrange
        var queryParams = "include=alternativeCodes&include=features&include=foo";

        // Act
        var actual = SearchCriteria.Parse<Item>(queryParams);

        // Assert
        actual.Include.Should().NotBeNull();
        actual.Include.Should().NotContain("foo"); // Skip invalid includes
        actual.Include.Should().BeEquivalentTo(new string[] { "AlternativeCodes", "Features" });
    }

    [Fact]
    public void Should_parse_sorting()
    {
        // Arrange
        var queryParams = "sort=-code&sort=description";

        // Act
        var actual = SearchCriteria.Parse<Item>(queryParams);

        // Assert
        actual.Sorting.Should().NotBeNull();

        var debugView = actual.Sorting.Select(s => s.ToString());
        var actualSorting = new string[]
        {
            "p => p.Code, Descending",
            "p => p.Description, Ascending"
        };
        debugView.Should().BeEquivalentTo(actualSorting);
    }

    [Fact]
    public void Should_parse_criteria()
    {
        // Arrange
        var queryParams = "id:gt=5&code=ATC-213&description:neq=test";

        // Act
        var actual = SearchCriteria.Parse<Item>(queryParams);

        // Assert
        actual.Specification.Should().NotBeNull();
        actual.Specification.ToString().Should().BeEquivalentTo("p => (((p.ID > 5) AndAlso (p.Code == ATC-213)) AndAlso (p.Description != \"test\"))");
    }

    [Fact]
    public void Should_parse_offset_paging()
    {
        // Arrange
        var queryParams = "offset=5&code=ATC-213&limit=20";

        // Act
        var actual = SearchCriteria.Parse<Item>(queryParams);

        // Assert
        actual.Paging.Should().NotBeNull();
        actual.Paging.Should().BeOfType<OffsetBasedPaging>();
        actual.Paging.Should().BeEquivalentTo(new OffsetBasedPaging() { Offset = 5, Limit = 20 });
    }
    
    [Fact]
    public void Should_parse_absolute_paging()
    {
        // Arrange
        var queryParams = "page_number=2&code=ATC-213&page_size=20";

        // Act
        var actual = SearchCriteria.Parse<Item>(queryParams);

        // Assert
        actual.Paging.Should().NotBeNull();
        actual.Paging.Should().BeOfType<AbsolutePaging>();
        actual.Paging.Should().BeEquivalentTo(new AbsolutePaging() { PageNumber = 2, PageSize = 20 });
    }
}
