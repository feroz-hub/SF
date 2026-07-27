/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class LdapProfileValidationException(string failureCode)
    : Exception("The LDAP profile could not be mapped.")
{
    public string FailureCode { get; } = failureCode;
}

public sealed class LdapProtocolException(string failureCode, Exception? innerException = null)
    : Exception("The LDAP operation failed.", innerException)
{
    public string FailureCode { get; } = failureCode;
}
