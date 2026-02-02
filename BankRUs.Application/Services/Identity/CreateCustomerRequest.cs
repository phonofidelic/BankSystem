using System;
using System.Collections.Generic;
using System.Text;

namespace BankRUs.Application.Services.Identity;

public sealed record CreateCustomerRequest
(
    Guid ApplicationUserId,
    string SocialSecurityNumber,
    string Email
);
