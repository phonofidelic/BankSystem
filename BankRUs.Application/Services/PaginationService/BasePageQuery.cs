using System.Web;

namespace BankRUs.Application.Services.PaginationService;

public record BasePageQuery(
    int Page = 1,
    int Size = 50,
    SortOrder Order = SortOrder.Descending)
{
    private readonly int _page = Page < 1 ? 1 : Page;
    private readonly int _size = (Size > 50 || Size < 1) ? 50 : Size;
    private int _offset { get => _page - 1; }
    public int Skip { get => _size * _offset; }

    public int MyProperty { get; set; }

    public static BasePageQuery Parse(string input)
    {
        var queryParams = HttpUtility.ParseQueryString(input.Split('?')[1]);
        if (queryParams == null)
        {
            return new BasePageQuery();
        }

        var pageString = queryParams.Get("page".Normalize());
        var sizeString = queryParams.Get("size".Normalize());
        var sortOrderString = queryParams.Get("order".Normalize());

        if (!int.TryParse(pageString, out int page)) page = 1;
        if (!int.TryParse (sizeString, out int size)) size = 50;
        if (!Enum.TryParse(sortOrderString, ignoreCase: true, out SortOrder sortOrder)) sortOrder = SortOrder.Descending;

        return new BasePageQuery(
            Page: page,
            Size: size,
            Order: sortOrder);
    }
}
