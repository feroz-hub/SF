namespace Zentra.Contracts.Responses;

public sealed record AuthTokenResponse(string AccessToken, string TokenType, long ExpiresIn);
