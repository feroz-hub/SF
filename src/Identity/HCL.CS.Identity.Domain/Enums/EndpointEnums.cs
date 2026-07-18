/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

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
