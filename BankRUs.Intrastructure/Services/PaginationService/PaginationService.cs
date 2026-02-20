using BankRUs.Application.Services.PaginationService;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Infrastructure.Services.PaginationService;


public class PaginationService : IPaginationService 
{
    private const int MAX_PAGE_SIZE = 50;
    public async Task<BasePagedResult<T>> GetPagedResultAsync<T>(BasePageQuery query, IQueryable<T> items)
    {
        int pageSize = query.Size < MAX_PAGE_SIZE ? query.Size : MAX_PAGE_SIZE;
        var totalItems = items.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        var result = items
            .Skip(query.Skip).Take(query.Size)
            ;

        return new BasePagedResult<T>
        (
            Items: await result.ToListAsync(),
            Paging: new PagedResultMetadata(
                Page: query.Page,
                PageSize: pageSize,
                TotalCount: totalItems,
                TotalPages: totalPages,
                Sort: query.SortOrder.ToString().ToLower())
        );
    }
}