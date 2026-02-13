using System;

namespace BankRUs.Api.Dtos.Me;

public record GetMeResponseDto(
    string UserId,
    string? Email
);
