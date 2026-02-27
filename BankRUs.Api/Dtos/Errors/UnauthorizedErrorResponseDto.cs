using System;

namespace BankRUs.Api.Dtos.Errors;

public record UnauthorizedErrorResponseDto() : BaseErrorResponseDto("Unauthorized");

