using BankRUs.Application.Services.PaginationService;

namespace BankRUs.Infrastructure.Services.PaginationService;


public class PaginationService : IPaginationService 
{
    private const int MAX_PAGE_SIZE = 50;
    public BasePagedResult<T> GetPagedResult<T>(BasePageQuery query, IQueryable<T> items)
    {
        int pageSize = query.Size < MAX_PAGE_SIZE ? query.Size : MAX_PAGE_SIZE;
        var totalItems = items.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var result = items
            .Skip(query.Skip).Take(query.Size)
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