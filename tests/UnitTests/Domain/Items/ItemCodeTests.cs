using FluentAssertions;
using SimpleAPI.Domain.Features.Items;

namespace SimpleAPI.UnitTests.Domain.Items;

public class ItemCodeTests
{
    [Fact]
    public void Should_create_objects()
    {
        var code = (ItemCode)"Test124";
        code.Should().NotBeNull();
        code.ToString().Should().Be("Test124");
    }

    [Fact]
    public void Should_compare()
    {
        ItemCode a = (ItemCode)"Code.523";
        ItemCode b = (ItemCode)"Code.123";

        (a > b).Should().BeTrue();
        (a >= b).Should().BeTrue();
        (a < b).Should().BeFalse();
        (a <= b).Should().BeFalse();
    }

    [Fact]
    public void Should_support_equality()
    {
        ItemCode a = (ItemCode)"Code.123";
        ItemCode b = (ItemCode)"Code.123";
        ItemCode c = (ItemCode)"Code.623";

        (a == b).Should().BeTrue();
        (a != c).Should().BeTrue();

    }
    
    [Fact]
    public void Should_throw_on_invalid_currency()
    {
        var lessThanThree = () => (ItemCode)"12";
        lessThanThree.Should().Throw<ArgumentException>()
            .WithMessage("Code length must be between 3 and 50 characters");
        
        var moreThanFifty = () => (ItemCode)"123456789012345678901234567890123456789012345678901234567890";
        lessThanThree.Should().Throw<ArgumentException>()
            .WithMessage("Code length must be between 3 and 50 characters");

        var nullInput = () => ItemCode.FromString(null!);
        nullInput.Should().Throw<ArgumentException>()
            .WithMessage("Input parameter cannot be null (Parameter 'code')");

    }
}
