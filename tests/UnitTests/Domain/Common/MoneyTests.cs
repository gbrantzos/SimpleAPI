using FluentAssertions;
using SimpleAPI.Domain.Features.Common;

namespace SimpleAPI.UnitTests.Domain.Common;

public class MoneyTests
{
    [Fact]
    public void Should_create_objects()
    {
        Money fromDecimal1 = 2.0M;
        fromDecimal1.Should().NotBeNull();

        Money fromDouble1 = 3.4;
        fromDouble1.Should().NotBeNull();

        Money fromDecimal2 = Money.FromDecimal(3.14M);
        fromDecimal2.Should().NotBeNull();

        Money fromDouble2 = Money.FromDouble(21);
        fromDouble2.Should().NotBeNull();

        Money inEuro = Money.InEuro(3.28M);
        inEuro.Should().NotBeNull();
        inEuro.ToString().Should().Be("3,28 EUR");
    }

    [Fact]
    public void Should_add()
    {
        Money a = 3.2;
        Money b = 2.1;

        (a + b).Should().Be(5.3);
        Money.Add(a, b).Should().Be(5.3);

        var action = () => a + Money.Create(3, Currency.USD);
        action.Should().Throw<ArgumentException>().WithMessage("Cannot add different currencies: EUR, USD");
    }

    [Fact]
    public void Should_subtract()
    {
        Money a = 3.2;
        Money b = 2.1;

        (a - b).Should().Be(1.1);
        Money.Subtract(a, b).Should().Be(1.1);
        
        var action = () => a - Money.Create(3, Currency.USD);
        action.Should().Throw<ArgumentException>().WithMessage("Cannot subtract different currencies: EUR, USD");

    }

    [Fact]
    public void Should_compare()
    {
        Money a = 3.2;
        Money b = 2.1;
        Money c = 5.3;

        (a > b).Should().BeTrue();
        (a >= b).Should().BeTrue();
        (a < b).Should().BeFalse();
        (a <= b).Should().BeFalse();
        ((a + b) == c).Should().BeTrue();
        (a != b).Should().BeTrue();
        (a > (Money)null!).Should().BeTrue();
        ((Money)null! > b).Should().BeFalse();
        
        // ReSharper disable once RedundantCast
        // ReSharper disable once EqualExpressionComparison
        ((Money)null! == (Money)null!).Should().BeTrue();
        
        var action = () => a >= Money.Create(3, Currency.USD);
        action.Should().Throw<ArgumentException>().WithMessage("Cannot compare Money values of different currencies!");
    }

    [Fact]
    public void Should_throw_on_invalid_currency()
    {
        var action = () => Money.Create(23, Currency.Invalid);
        action.Should().Throw<ArgumentException>().WithMessage("Invalid currency!");
    }
}
