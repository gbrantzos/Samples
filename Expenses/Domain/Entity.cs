namespace Expenses.Domain
{
    public abstract class Entity
    {
        public long Id { get; private set; } = KeyGenerator.NewId();
    }
}
