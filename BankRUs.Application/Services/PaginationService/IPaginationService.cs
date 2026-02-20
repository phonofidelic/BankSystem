namespace BankRUs.Application.Services.PaginationService;

public interface IPaginationService
{
    public Task<BasePagedResult<T>> GetPagedResultAsync<T>(BasePageQuery query, IQueryable<T> items);
}
