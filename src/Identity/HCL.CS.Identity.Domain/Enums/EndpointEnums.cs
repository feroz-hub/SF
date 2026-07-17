namespace HCL.CS.Domain.Enums;

public enum ApplicationType
{
    RegularWeb = 1,

    SinglePageApp = 2,

    Native = 3,

    Service = 4
}

public enum GrantType
{
    AuthorizationCode = 1,

    Password = 2,

    ClientCredentials = 3,

    RefreshToken = 4
}

public enum AccessTokenType
{
    JWT = 1 // TODO: Enum JWT is never used.
    // Reference = 2  // Planned for V2 Release.
}

public enum CspLevel
{
    One = 0,

    Two = 1
}

public enum SigningAlgorithm
{
    RS256 = 1,

    RS384 = 2,

    RS512 = 3,

    HS256 = 4,

    HS384 = 5,

    HS512 = 6,

    ES256 = 7,

    ES384 = 8,

    ES512 = 9,

    PS256 = 10,

    PS384 = 11,

    PS512 = 12
}

public enum ParseMethods
{
    Basic = 0,

    Post = 1,

    JwtSecret = 2
}
