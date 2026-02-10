using BankRUs.Application;
using BankRUs.Application.Exceptions;
using BankRUs.Application.Services.CustomerService;
using BankRUs.Application.Services.CustomerService.GetCustomer;
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

        public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                var newCustomer = new Customer
                {
                    Id = Guid.NewGuid(),
                    //ApplicationUserId = request.ApplicationUserId,
                    Email = request.Email,
                    SocialSecurityNumber = request.SocialSecurityNumber
                };

                await _context.Customers.AddAsync(newCustomer);
                //await _context.SaveChangesAsync();

                return new CreateCustomerResult(newCustomer);

            } catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetCustomerIdResult> GetCustomerIdAsync(GetCustomerIdRequest request)
        {
            var customer = await _context
                .Customers.Where(customer => customer.ApplicationUserId == request.ApplicationUserId)
                .FirstAsync() ?? throw new CustomerNotFoundException(string.Format("Customer not found with user Id {0}", request.ApplicationUserId));

            return new GetCustomerIdResult(CustomerId: customer.Id);
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
                //await _context.SaveChangesAsync();
                return new CreateBankAccountResult(newBankAccount);
            }
            catch
            {
                throw;
            }
        }
    }
}
