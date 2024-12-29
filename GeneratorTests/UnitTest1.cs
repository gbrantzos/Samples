using FluentAssertions;

namespace GeneratorTests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var input = """
                    using SimpleAPI.Core;

                    namespace SimpleAPI.Domain.Features.Items;
                    {
                        [StronglyTypedID]
                        public class Item
                        {
                            public string Name { get; set; }
                        }
                    }
                    """;

        var actual = Helpers.GetGeneratedOutput(input);
        actual.Should().NotBeNull();
        actual.Count.Should().Be(2);
    }
}