using System;

namespace BankSystem.Api.Dtos.Errors;

public record BadRequestResponseDto(string? Message = null) : BaseErrorResponseDto(Message ?? "Bad request")
{

}
