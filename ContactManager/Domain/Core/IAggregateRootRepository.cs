namespace ContactManager.Domain.Core
{
    public interface IAggregateRootRepository
    {
        long NextID();
    }
}
