namespace HCL.CS.Contracts.Responses;

public sealed record AuthTokenResponse(string AccessToken, string TokenType, long ExpiresIn);
