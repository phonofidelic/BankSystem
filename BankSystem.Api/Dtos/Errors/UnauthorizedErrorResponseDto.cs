using System;

namespace BankSystem.Api.Dtos.Errors;

public record UnauthorizedErrorResponseDto() : BaseErrorResponseDto("Unauthorized");

