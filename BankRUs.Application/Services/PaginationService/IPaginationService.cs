namespace BankRUs.Application.Services.PaginationService;

public interface IPaginationService
{
    public BasePagedResult<T> GetPagedResult<T>(BasePageQuery query, IQueryable<T> items);
}
