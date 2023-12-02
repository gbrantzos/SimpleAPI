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

    [Fact]
    public void Should_change_code_to_different()
    {
        // Act
        var newCode = "TEST.0098";
        var result = _item.ChangeCode(newCode);

        // Assert
        result.Should().BeTrue();
        _item.Code.Should().Be((ItemCode)newCode);

        // Make sure new base code exists as alternative
        var asAlternative = _item.AlternativeCodes
            .Single(c => c.Kind == ItemAlternativeCode.CodeKind.Base);
        asAlternative.Code.Should().Be((ItemCode)newCode);

        // Make sure that new code does not exist as alternative
        _item.AlternativeCodes
            .Any(c => c.Code == newCode && c.Kind == ItemAlternativeCode.CodeKind.Alternative)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Should_fail_to_change_code_to_used()
    {
        // Arrange
        _item.AddAlternativeCode("TST.978");
        
        // Act
        var newCode = "TST.978";
        var result = _item.ChangeCode(newCode);

        // Assert
        result.Should().BeFalse();
    }
}
