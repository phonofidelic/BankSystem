using System;
using System.Collections.Generic;
using System.Text;

namespace BankSystem.Application
{
    public interface IHandler<TRequest,TResult>
    {
        public Task<TResult> HandleAsync(TRequest request);
    }
}
