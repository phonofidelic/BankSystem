using System;

namespace BankSystem.Api.Dtos.Me;

public record GetMeResponseDto(
    string UserId,
    string? Email
);
