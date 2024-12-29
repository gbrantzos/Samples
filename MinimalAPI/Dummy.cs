using SimpleAPI.Core;

namespace MinimalAPI;

public static class Dummy
{
    public static IEndpointRouteBuilder MapDummy(this IEndpointRouteBuilder builder)
    {
        builder.Map("/dummy", DummyHandler)
            .WithDescription("test me")
            .WithName("dummy")
            .Produces<Game>(200);

        return builder;
    }

    private static Task DummyHandler(HttpContext context, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

[StronglyTypedID]
public class TestMe: Entity
{
    public TestMeID Id { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; } = String.Empty;
}