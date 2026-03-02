using System;

namespace BankRUs.Api.Dtos.Errors;

public record NotFoundErrorResponseDto(string? Message = null) : BaseErrorResponseDto(Message ?? "Not found");
