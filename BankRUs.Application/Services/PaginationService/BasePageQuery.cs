using System.Web;

namespace BankRUs.Application.Services.PaginationService;

public record BasePageQuery(
    int Page = 1,
    int Size = 50,
    SortOrder SortOrder = SortOrder.Descending)
{
    private int _offset { get => Page - 1; }
    public int Skip { get => Size * _offset; }

    public static BasePageQuery Parse(string input)
    {
        var queryParams = HttpUtility.ParseQueryString(input.Split('?')[1]);

        var pageString = queryParams.Get("page".Normalize());
        var sizeString = queryParams.Get("size".Normalize());
        var sortOrderString = queryParams.Get("sortOrder".Normalize());

        if (!int.TryParse(pageString, out int page)) page = 1;
        if (!int.TryParse (sizeString, out int size)) size = 50;
        if (!SortOrder.TryParse(sortOrderString, ignoreCase: true, out SortOrder sortOrder)) sortOrder = SortOrder.Descending;

        return new BasePageQuery(
            Page: page,
            Size: size,
            SortOrder: sortOrder);
    }
}
