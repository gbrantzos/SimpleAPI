using FluentAssertions;
using SimpleAPI.Domain.Base;

namespace SimpleAPI.UnitTests.Domain.Base;

public class EntityTests
{
    private class SimpleID : IEntityID
    {
        public int Value { get; init; }
        public bool IsNew { get; } = false;
    }

    [Fact]
    public void Should_create()
    {
        var id = new SimpleID() { Value = 23};
        
        id.Should().NotBeNull();
        id.Value.Should().Be(23);
    }
}
