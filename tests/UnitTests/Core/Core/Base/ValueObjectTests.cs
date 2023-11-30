using FluentAssertions;
using SimpleAPI.Core.Base;

namespace SimpleAPI.UnitTests.Core.Core.Base;

public class ValueObjectTests
{
    private sealed class SampleValueObject : ValueObject
    {
        public long ID { get; set; }
        public long Sequence { get; set; }
        
        protected override IEnumerable<IComparable> GetEqualityComponents()
        {
            yield return ID;
            yield return Sequence;
        }
    }
    
    [Fact]
    public void Same_values_are_equal()
    {
        // Arrange
        var vo1 = new SampleValueObject() { ID = 1, Sequence = 3 };
        var vo2 = new SampleValueObject() { ID = 1, Sequence = 3 };
        
        // Act 
        var compare = vo1 == vo2;
        
        // Arrange
        compare.Should().BeTrue();
        vo1.GetHashCode().Should().Be(vo2.GetHashCode());
    }
    
    [Fact]
    public void Null_values_are_not_equal()
    {
        // Arrange
        var vo1 = new SampleValueObject() { ID = 1, Sequence = 3 };
        var vo2 = (SampleValueObject)null!;
        
        // Act 
        var compareLeft = vo1 == vo2;
        var compareRight = vo2 == vo1;
        
        // Arrange
        compareLeft.Should().BeFalse();
        compareRight.Should().BeFalse();
    }

}
