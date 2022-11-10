namespace DDD.Domain.Core;

public abstract class EntityID : ValueObject
{
    public int Value { get; init; }

    protected EntityID() { }

    protected EntityID(int value) => Value = value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{GetType().Name}, #{Value}";
}

public abstract class Entity<TID> where TID : EntityID, new()
{
    public TID? ID { get; init; }
    public byte[]? Version { get; set; }

    public bool IsNew => ID is null;
}