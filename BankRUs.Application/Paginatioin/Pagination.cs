using System.ComponentModel.DataAnnotations;

namespace BankRUs.Application.Paginatioin;


public static class Pagination {
    private const int MAX_PAGE_SIZE = 50;
    public static BasePagedResult<T> GetPagedResult<T>(BasePageQuery query, IQueryable<T> items)
    {
        int pageSize = query.PageSize < MAX_PAGE_SIZE ? query.PageSize : MAX_PAGE_SIZE;
        var totalItems = items.Count();
        var totalPages = (totalItems / pageSize) + 1;

        var result = items
            .Skip(query.Skip).Take(query.PageSize)
            .ToList();

        return new BasePagedResult<T>
        (
            Items: result,
            Meta: new PagedResultMetadata(
                Page: query.Page,
                PageSize: pageSize,
                TotalCount: totalItems,
                TotalPages: totalPages,
                Sort: query.SortOrder.ToString().ToLower())
        );
    }
}