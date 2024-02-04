using FluentAssertions;

namespace SimpleAPI.UnitTests.Generator;

public class StronglyTypedIdsTests
{
    [Fact]
    public void Should_create_EntityID_from_class()
    {
        var input = """
                    using SimpleAPI.Core;

                    namespace SimpleAPI.Domain;
                    {
                        [StronglyTypedID]
                        public class Item
                        {
                            public string Name { get; set; }
                        }
                    }
                    """;

        var expectedAttributes = """
                                 namespace SimpleAPI.Core
                                 {
                                     [System.AttributeUsage(System.AttributeTargets.Class)]
                                     public class StronglyTypedIDAttribute : System.Attribute { }
                                 }
                                 """;
        
        var expectedTypeIDs = """

                              using SimpleAPI.Domain.Base;
                              using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

                              namespace SimpleAPI.Domain;

                              public readonly struct ItemID : IComparable<ItemID>, IEquatable<ItemID>, IEntityID
                              {
                                  public int Value { get; }
                                  public bool IsNew => Value == 0;
                              
                                  public ItemID(int value) => Value = value;
                                  
                                  public bool Equals(ItemID other) => this.Value.Equals(other.Value);
                                  public int CompareTo(ItemID other) => Value.CompareTo(other.Value);
                              
                                  public override bool Equals(object? obj)
                                  {
                                      if (ReferenceEquals(null, obj)) return false;
                                      return obj is ItemID other && Equals(other);
                                  }
                              
                                  public override int GetHashCode() => Value.GetHashCode();
                                  public override string ToString() => Value.ToString();
                              
                                  public static bool operator ==(ItemID a, ItemID b) => a.CompareTo(b) == 0;
                                  public static bool operator !=(ItemID a, ItemID b) => !(a == b);
                                  
                                  public static bool operator <(ItemID left, ItemID right) => left.CompareTo(right) < 0;
                                  public static bool operator <=(ItemID left, ItemID right) => left.CompareTo(right) <= 0;
                                  public static bool operator >(ItemID left, ItemID right) => left.CompareTo(right) > 0;
                                  public static bool operator >=(ItemID left, ItemID right) => left.CompareTo(right) >= 0;
                                  
                                  public class EFValueConverter : ValueConverter<ItemID, int>
                                  {
                                      public EFValueConverter() : base(
                                         v => v.Value,
                                         v => new ItemID(v)
                                     ) { }
                                  }
                              }
                              """;
        

        var actual = GeneratorTestsHelper.GetGeneratedOutput(input);
        actual.Should().NotBeNull();
        actual.Count.Should().Be(2);
        actual[0].Should().Be(expectedAttributes);
        
        // Lets skip comments
        var generatedCode = actual[1]
            .Split(Environment.NewLine)
            .Where(l => !l.StartsWith("//") && !l.StartsWith("#"))
            .ToArray();
        String.Join(Environment.NewLine, generatedCode).Should().Be(expectedTypeIDs);
    }
}
