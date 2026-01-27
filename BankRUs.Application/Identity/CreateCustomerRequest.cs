using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Identity;

public sealed record CreateCustomerRequest
(
    Guid ApplicationUserId
);
