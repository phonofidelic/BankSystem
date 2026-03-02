using System;

namespace BankRUs.Api.Dtos.Errors;

public record BadRequestResponseDto(string? Message = null) : BaseErrorResponseDto(Message ?? "Bad request")
{

}
