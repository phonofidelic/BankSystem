using BankRUs.Application.Paginatioin;

namespace BankRUs.Application.Services.CustomerService;

public record CustomersPageQuery(int Page, int PageSize, SortOrder SortOrder) : BasePageQuery(Page, PageSize, SortOrder);
