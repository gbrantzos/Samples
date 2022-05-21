namespace LRUCache
{
    public interface ILRUCache<T>
    {
        void Add(string key, T value);
        T Get(string key);
    }
}