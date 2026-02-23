namespace BankRUs.Application.Services.PaginationService;

public interface IPaginationService
{
    public Task<BasePagedResult<TItem>> GetPagedResultAsync<TItem>(BasePageQuery query, IQueryable<TItem> items);
}
