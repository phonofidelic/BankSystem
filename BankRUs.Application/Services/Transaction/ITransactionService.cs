using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.TransactionService
{
    public interface ITransactionService
    {
        public Task<CreateTransactionResult> CreateTransactionAsync(CreateTransactionRequest request);
    }
}
