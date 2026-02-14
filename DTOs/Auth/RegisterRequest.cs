namespace PayByLink.DTOs.Auth;

public record RegisterRequest(
    string Name,
    string Email,
    string Password,
    string? CompanyName,
    string Country
);
