namespace PayByLink.DTOs.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken
);
