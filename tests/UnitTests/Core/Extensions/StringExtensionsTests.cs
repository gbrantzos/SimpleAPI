using FluentAssertions;
using SimpleAPI.Core;

namespace SimpleAPI.UnitTests.Core.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("ItemID", "item_id")]
    [InlineData("OrderedItemID", "ordered_item_id")]
    [InlineData("orderedItemID", "ordered_item_id")]
    [InlineData("Item", "item")]
    [InlineData("ItemID123", "item_id123")]
    [InlineData("_ItemID", "_item_id")]
    [InlineData("", "")]
    [InlineData("12items", "12items")]
    [InlineData("12ItemsSold", "12items_sold")]
    public void Should_convert_input_to_snake_case(string sample, string expected)
    {
        var actual = sample.ToSnakeCase();
        actual.Should().Be(expected);
    }
}
