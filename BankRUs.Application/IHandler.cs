using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application
{
    public interface IHandler<TRequest,TResult>
    {
        public Task<TResult> HandleAsync(TRequest request);
    }
}
