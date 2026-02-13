using BankRUs.Application.Configuration;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.UseCases.CustomerServiceRep.ListCustomerAccounts;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BankRUs.Infrastructure.Services.CustomerService
{
    public class CustomerService(
        IOptions<AppSettings> appSettings,
        ApplicationDbContext context) : ICustomerService
    {
        private readonly AppSettings _appSettings = appSettings.Value;
        private readonly ApplicationDbContext _context = context;

        public async Task<IQueryable<Customer>> SearchCustomersAsync(ListCustomerAccountsQuery query)
        {
            var search = query.Search ?? string.Empty;
            var results = _context.Customers
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

        public async Task<Customer> GetCustomerAsync(Guid customerId)
        {
            return await _context.Customers.Include(c => c.BankAccounts)
                .Where(c => c.Id == customerId)
                .FirstOrDefaultAsync() ?? throw new CustomerNotFoundException();
        }

        public async Task<Guid> GetCustomerIdAsync(Guid applicationUserId)
        {
            var customer = await _context
                .Customers.Where(customer => customer.ApplicationUserId == applicationUserId)
                .FirstAsync() ?? throw new CustomerNotFoundException(string.Format("Customer not found with user Id {0}", applicationUserId));

            return customer.Id;
        }

        public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                var newCustomer = new Customer
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    SocialSecurityNumber = request.SocialSecurityNumber
                };

                await _context.Customers.AddAsync(newCustomer);

                return new CreateCustomerResult(newCustomer);

            } catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CreateBankAccountResult> CreateBankAccountAsync(CreateBankAccountRequest request)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(request.CustomerId) 
                    ?? throw new Exception("Customer not found");
                
                BankAccount newBankAccount = new()
                {
                    Name = request.BankAccountName,
                    CustomerId = customer.Id,
                    Currency = _appSettings.DefaultCurrency
                };

                _context.BankAccounts.Add(newBankAccount);
                return new CreateBankAccountResult(newBankAccount);
            }
            catch
            {
                throw;
            }
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
    }
}
