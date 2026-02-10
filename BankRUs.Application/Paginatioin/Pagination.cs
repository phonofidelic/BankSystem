using System.ComponentModel.DataAnnotations;

namespace BankRUs.Application.Paginatioin;


public static class Pagination {
    public static BasePagedResult<T> GetPagedResult<T>(BasePageQuery query, IQueryable<T> items)
    {
        var totalItems = items.Count();
        var totalPages = (totalItems / query.PageSize) + 1;

        var result = items
            .Skip(query.Skip).Take(query.PageSize)
            .ToList();

        return new BasePagedResult<T>
        (
            Items: result,
            Meta: new PagedResultMetadata(
                Page: query.Page,
                PageSize: query.PageSize,
                TotalCount: totalItems,
                TotalPages: totalPages,
                Sort: query.SortOrder.ToString().ToLower())
        );
    }
}