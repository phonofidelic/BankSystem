using BankRUs.Application.Configuration;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerAccountService;
using BankRUs.Application.UseCases.ListCustomerAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Domain.ValueObjects;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.CustomerAccountService
{
    public class CustomerAccountService(
        IOptions<AppSettings> appSettings,
        ApplicationDbContext context) : ICustomerAccountService
    {
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly ApplicationDbContext _context = context;

        public async Task<IQueryable<CustomerAccount>> SearchCustomerAccountsAsync(ListCustomerAccountsPageQuery query)
        {
            var search = query.Search ?? string.Empty;
            var results = _context.Customers.AsNoTracking()
                .Where(c =>
                      c.FirstName.Contains(search)
                    | c.LastName.Contains(search)
                    | c.Email.Contains(search)
                    | c.SocialSecurityNumber.Contains(search))
                .Where(c =>
                       c.FirstName.Contains(query.FirstName ?? string.Empty)
                    && c.LastName.Contains(query.LastName ?? string.Empty)
                    && c.Email.Contains(query.Email ?? string.Empty)
                    && c.SocialSecurityNumber.Contains(query.Ssn ?? string.Empty))
                .AsQueryable();

            return results;
        }

        public async Task<CustomerAccount> GetCustomerAccountAsync(Guid customerId)
        {
            return await _context.Customers.Include(c => c.BankAccounts)
                .Where(c => c.Id == customerId)
                .FirstOrDefaultAsync() ?? throw new CustomerNotFoundException();
        }

        public async Task<Guid> GetCustomerAccountIdAsync(Guid applicationUserId)
        {
            var customer = await _context
                .Customers.Where(customer => customer.ApplicationUserId == applicationUserId)
                .FirstOrDefaultAsync() ?? throw new CustomerNotFoundException(string.Format("Customer not found with user Id {0}", applicationUserId));

            return customer.Id;
        }

        public async Task<CustomerAccount?> GetClosedCustomerAccountBySocialSecurityNumber(string socialSecurityNumber)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => 
                c.SocialSecurityNumber == socialSecurityNumber
                && c.Status == CustomerAccountStatus.Closed);
        }

        public bool EmailExists(string email)
        {
            var result = _context.Customers.Where(c => c.Email == email).FirstOrDefault();
            return result != null;
        }

        public bool SsnExists(string ssn)
        {
            var result = _context.Customers.Where(c => c.SocialSecurityNumber == ssn).FirstOrDefault();
            return result != null;
        }

        public CompleteCustomerAccountDetails ValidateCustomerAccountDetails(CustomerAccountDetails details)
        {
            var firstName = details.FirstName ?? throw new CustomerAccountDetailsValidationException();
            var lastName = details.LastName ?? throw new CustomerAccountDetailsValidationException();
            var email = details.Email ?? throw new CustomerAccountDetailsValidationException();
            var socialSecurityNumber = details.SocialSecurityNumber ?? throw new CustomerAccountDetailsValidationException();

            return new CompleteCustomerAccountDetails(firstName, lastName, email, socialSecurityNumber);
        }
    }
}
