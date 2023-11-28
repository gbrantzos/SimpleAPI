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
                                     public class StronglyTypedID : System.Attribute { }
                                 }
                                 """;
        
        var expectedTypeIDs = """
                              //------------------------------------------------------------------------------
                              // <auto-generated>
                              //     This code was generated by source generator
                              //
                              //     Changes to this file may cause incorrect behavior and will be lost if
                              //     the code is regenerated.
                              // </auto-generated>
                              //------------------------------------------------------------------------------
                              #nullable enable

                              using SimpleAPI.Domain.Base;
                              using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

                              namespace SimpleAPI.Base;

                              public readonly struct ItemID : IComparable<ItemID>, IEquatable<ItemID>, IEntityID
                              {
                                  public int Value { get; }
                                  
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
        actual[1].Should().Be(expectedTypeIDs);
    }
}
