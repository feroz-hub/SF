/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.DirectoryServices.Protocols;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public interface ILdapConnectionFactory
{
    LdapConnection Create();
}

public sealed class SystemDirectoryServicesLdapConnectionFactory(HclCsConfig configuration)
    : ILdapConnectionFactory
{
    private readonly LdapConfig ldapConfig = configuration.SystemSettings.LdapConfig;

    public LdapConnection Create()
    {
        var identifier = new LdapDirectoryIdentifier(
            ldapConfig.LdapHostName,
            ldapConfig.LdapPort,
            false,
            false);
        var connection = new LdapConnection(identifier)
        {
            Timeout = TimeSpan.FromSeconds(ldapConfig.ConnectTimeoutSeconds)
        };
        connection.SessionOptions.ProtocolVersion = 3;
        connection.SessionOptions.SecureSocketLayer = ldapConfig.IsSslEnabled;

        // System.DirectoryServices.Protocols performs normal OS trust-chain and hostname validation
        // when VerifyServerCertificate is not overridden. Do not add a permissive callback here.
        return connection;
    }
}
