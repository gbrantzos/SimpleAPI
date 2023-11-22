using FluentAssertions;

namespace SimpleAPI.UnitTests.Generator;

public class StronglyTypedIdsTests
{
    [Fact]
    public void Should_create_EntityID_from_class()
    {
        var input = """
                    using SimpleAPI.Generator;
                    
                    namespace SimpleAPI.Base;
                    {
                        [HasStronglyTypedID]
                        public class Item
                        {
                            public string Name { get; set; }
                        }
                    }
                    """;
        
        var expected = """
                       using SimpleAPI.Domain.Base;

                       namespace SimpleAPI.Base;

                       public readonly struct ItemID : IComparable<ItemID>, IEquatable<ItemID>, IEntityID
                       {
                           public int Value { get; }
                       
                           public ItemID(int value) => Value = value;
                           
                           
                           public bool Equals(ItemID other) => this.Value.Equals(other.Value);
                           public int CompareTo(ItemID other) => Value.CompareTo(other.Value);
                       
                           public override bool Equals(object obj)
                           {
                               if (ReferenceEquals(null, obj)) return false;
                               return obj is ItemID other && Equals(other);
                           }
                       
                           public override int GetHashCode() => Value.GetHashCode();
                           public override string ToString() => Value.ToString();
                       
                           public static bool operator ==(ItemID a, ItemID b) => a.CompareTo(b) == 0;
                           public static bool operator !=(ItemID a, ItemID b) => !(a == b);
                       }
                       """;
        var actual = GeneratorTestsHelper.GetGeneratedOutput(input);
        
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }
}
