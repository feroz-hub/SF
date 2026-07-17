using Zentra.DemoClientMvc.ViewModels.Shared;

namespace Zentra.DemoClientMvc.ViewModels.Token;

public sealed class TokenViewModel
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public string? IdToken { get; set; }

    public string? ExpiresAtUtc { get; set; }

    public IReadOnlyCollection<ClaimItemViewModel> AccessTokenClaims { get; set; } = new List<ClaimItemViewModel>();

    public string MaskedAccessToken => Mask(AccessToken);

    public string MaskedRefreshToken => Mask(RefreshToken);

    public string MaskedIdToken => Mask(IdToken);

    public bool HasTokens =>
        !string.IsNullOrWhiteSpace(AccessToken) ||
        !string.IsNullOrWhiteSpace(RefreshToken) ||
        !string.IsNullOrWhiteSpace(IdToken);

    private static string Mask(string? rawToken)
    {
        if (string.IsNullOrWhiteSpace(rawToken)) return "(not available)";

        return rawToken.Length <= 16
            ? "********"
            : string.Concat(rawToken.AsSpan(0, 8), "...", rawToken.AsSpan(rawToken.Length - 8));
    }
}
