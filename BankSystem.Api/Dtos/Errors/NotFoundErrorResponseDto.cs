using System;

namespace BankSystem.Api.Dtos.Errors;

public record NotFoundErrorResponseDto(string? Message = null) : BaseErrorResponseDto(Message ?? "Not found");
