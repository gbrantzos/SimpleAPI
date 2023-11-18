using FluentAssertions;

namespace SimpleAPI.UnitTests.Generator;

public class StronglyTypedIdsTests
{
    [Fact]
    public void Generator_Tests()
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

                       public class ItemID : EntityID
                       {
                           public ItemID(int id) : base(id) { }
                           public ItemID() : base(0) { }
                       }
                       """;
        var actual = GeneratorTestsHelper.GetGeneratedOutput(input);
        
        actual.Should().NotBeNull();
        actual.Should().Be(expected);
    }
}
