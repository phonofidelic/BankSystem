using BankRUs.Application.Services.CustomerService;
using BankRUs.Domain.Entities;
using BankRUs.Infrastructure.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Infrastructure.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreateCustomerResult> CreateCustomerAsync(CreateCustomerRequest request)
        {
            try
            {
                var newCustomer = new Customer
                {
                    Id = Guid.NewGuid(),
                    ApplicationUserId = request.ApplicationUserId,
                    Email = request.Email,
                    SocialSecurityNumber = request.SocialSecurityNumber
                };

                await _context.Customers.AddAsync(newCustomer);
                await _context.SaveChangesAsync();

                return new CreateCustomerResult(newCustomer.Id);

            } catch (Exception ex)
            {
                throw new Exception("Could not create Customer");
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
                    CustomerId = customer.Id
                };

                _context.BankAccounts.Add(newBankAccount);
                await _context.SaveChangesAsync();
                return new CreateBankAccountResult(newBankAccount);
            }
            catch
            {
                throw;
            }
        }
    }
}
