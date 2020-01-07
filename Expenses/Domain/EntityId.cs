
namespace Expenses.Domain
{
    public abstract class EntityId
    {
        public long Value { get; protected set; } = KeyGenerator.NewId();

        public override int GetHashCode() => Value.GetHashCode();
        public override string ToString() => $"{GetType().Name}, {Value.ToString()}";
    }
}