namespace Zentra.Contracts.Requests;

public sealed record AuthTokenRequest(string ClientId, string ClientSecret, string Scope);
