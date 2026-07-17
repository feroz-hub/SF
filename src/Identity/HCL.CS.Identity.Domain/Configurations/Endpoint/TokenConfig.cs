namespace HCL.CS.Domain.Configurations.Endpoint;

public class TokenConfig
{
    public string IssuerUri { get; set; }

    // Canonical API audience that resource servers must enforce.
    public string ApiIdentifier { get; set; }

    public int CachingLifetime { get; set; } = 3600;

    public bool ShowKeySet { get; set; } = true;

    public int TokenExpiration { get; set; } = 60;

    public int ClientSecretLength { get; set; } = 256;

    public int ClientSecretExpirationInDays { get; set; } = 60;
}

// TODO To implement feature to disable endpoint when set to false

public class EndpointsConfig
{
    public bool EnableAuthorizeEndpoint { get; set; } = true;

    public bool EnableJWKSEndpoint { get; set; } = true;

    public bool EnableTokenEndpoint { get; set; } = true;

    public bool EnableUserInfoEndpoint { get; set; } = true;

    public bool EnableDiscoveryEndpoint { get; set; } = true;

    public bool EnableEndSessionEndpoint { get; set; } = true;

    public bool EnableTokenRevocationEndpoint { get; set; } = true;

    public bool EnableIntrospectionEndpoint { get; set; } = true;

    public bool FrontchannelLogoutSupported { get; set; } = false;

    public bool FrontchannelLogoutSessionRequired { get; set; } = false;

    public bool BackchannelLogoutSupported { get; set; } = false;

    public bool BackchannelLogoutSessionRequired { get; set; } = false;
}

public class TokenExpiration
{
    public int MinAccessTokenExpiration { get; set; } = 60;

    public int MaxAccessTokenExpiration { get; set; } = 900;

    public int MinIdentityTokenExpiration { get; set; } = 60;

    public int MaxIdentityTokenExpiration { get; set; } = 3600;

    public int MinRefreshTokenExpiration { get; set; } = 300;

    public int MaxRefreshTokenExpiration { get; set; } = 86400;

    public int MinAuthorizationCodeExpiration { get; set; } = 60;

    public int MaxAuthorizationCodeExpiration { get; set; } = 600;

    public int MinLogoutTokenExpiration { get; set; } = 1800;

    public int MaxLogoutTokenExpiration { get; set; } = 86400;
}
