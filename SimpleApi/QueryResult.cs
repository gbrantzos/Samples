namespace SimpleApi;

public record QueryResult<T>
{
    public IEnumerable<T> Items { get; }
    public long Count { get; }

    public QueryResult(IEnumerable<T> items)
    {
        var list = items.ToList();
        Items = list;
        Count = list.Count;
    }
}
