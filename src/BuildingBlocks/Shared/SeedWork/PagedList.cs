namespace Shared.SeedWork;

public class PagedList<T>: List<T>
{
    public PagedList(IEnumerable<T> items, long totalItems, int currentPage, int pageSize)
    {
        _metaData = new MetaData()
        {
            TotalItems = totalItems,
            PageSize = pageSize,
            CurrentPage = currentPage,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
        };
        AddRange(items);
    }

    private MetaData _metaData { get; }

    public MetaData GetMetaData()
    {
        return _metaData;
    }
}