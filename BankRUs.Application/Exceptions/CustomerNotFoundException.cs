namespace BankRUs.Application.Exceptions;

public class CustomerNotFoundException : NotFoundException
{
    public CustomerNotFoundException() : base(message: "Customer not found")
    {}
    public CustomerNotFoundException(string message) : base(message) 
    {}
}
