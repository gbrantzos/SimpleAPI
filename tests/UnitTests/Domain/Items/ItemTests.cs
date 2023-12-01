using FluentAssertions;
using SimpleAPI.Domain.Features.Common;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Domain.Items;

public class ItemTests
{
    private readonly Item _item = Item.Create("CODE.001", "Test");
    
    [Fact]
    public void Should_create()
    {
        // Assert
        _item.Should().NotBeNull();
    }

    [Fact]
    public void Should_add_main_code_as_base_alternative()
    {
        // Assert
        _item.Should().NotBeNull();
        _item.AlternativeCodes
            .Should()
            .Contain(a => a.Code == "CODE.001" && a.Kind == ItemAlternativeCode.CodeKind.Base);
    }

    [Fact]
    public void Should_add_new_alternative()
    {
        // Act
        _item.AddAlternativeCode("ALTER.098", "New alternative code");

        // Assert
        var alternativeCode = _item.GetAlternativeCode("ALTER.098");
        alternativeCode.Should().NotBeNull();
        alternativeCode!.Description.Should().Be("New alternative code");
        alternativeCode.Kind.Should().Be(ItemAlternativeCode.CodeKind.Alternative);
    }

    [Fact]
    public void Should_find_and_remove_existing_alternative_code()
    {
        // Arrange
        _item.AddAlternativeCode("ALTER.497", "Alternative code");

        // Act
        _item.RemoveAlternativeCode("ALTER.497");

        // Arrange
        var actual = _item.GetAlternativeCode("ALTER.497");
        actual.Should().BeNull();
    }

    [Fact]
    public void Should_change_price()
    {
        // Arrange
        _item.SetPrice(3.14);

        // Act
        var newPrice = Money.InEuro(4.89m);
        _item.SetPrice(newPrice);

        // Assert
        _item.Price.Should().Be(newPrice);
    }
}
