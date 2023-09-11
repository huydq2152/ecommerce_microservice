namespace Shared.SeedWork;

public class PagingRequestParameters
{
    private const int MaxPageSize = 50;

    private int _currentPage = 1;

    private int _pageSize = 10;

    public int CurrentPage
    {
        get => _currentPage;
        set => _currentPage = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value > 0)
                _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
    
    public string OrderBy { get; set; }
}