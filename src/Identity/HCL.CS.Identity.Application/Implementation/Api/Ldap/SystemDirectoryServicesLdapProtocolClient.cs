/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Authentication;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;

namespace HCL.CS.Service.Implementation.Api.Ldap;

public sealed class SystemDirectoryServicesLdapProtocolClient(
    HclCsConfig configuration,
    ILdapConnectionFactory connectionFactory)
    : ILdapProtocolClient
{
    private const int InvalidCredentialsErrorCode = 49;
    private readonly LdapConfig ldapConfig = configuration.SystemSettings.LdapConfig;

    public async Task<IReadOnlyList<LdapDirectoryEntry>> SearchUsersAsync(
        string escapedIdentifier,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(escapedIdentifier);

        var filter = ldapConfig.UserSearchFilter.Replace(
            "{0}",
            escapedIdentifier,
            StringComparison.Ordinal);
        return await SearchAsync(filter, cancellationToken);
    }

    public async Task<IReadOnlyList<LdapDirectoryEntry>> SearchByImmutableIdAsync(
        string directoryImmutableId,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directoryImmutableId);

        var escapedValue = Guid.TryParse(directoryImmutableId, out var objectGuid)
            ? LdapFilterEncoder.EscapeBytes(objectGuid.ToByteArray())
            : LdapFilterEncoder.Escape(directoryImmutableId.Trim());
        var filter = $"({ldapConfig.Attributes.ImmutableId}={escapedValue})";
        return await SearchAsync(filter, cancellationToken);
    }

    public async Task ValidateCredentialsAsync(
        string distinguishedName,
        string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(distinguishedName);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        await ExecuteAsync(
            connection =>
            {
                PrepareTransport(connection);
                connection.AuthType = AuthType.Basic;
                connection.Bind(new NetworkCredential(distinguishedName, password));
                return true;
            },
            TimeSpan.FromSeconds(ldapConfig.ConnectTimeoutSeconds),
            true,
            cancellationToken);
    }

    private async Task<T> ExecuteAsync<T>(
        Func<LdapConnection, T> operation,
        TimeSpan timeout,
        bool isCredentialBind,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        using var connection = connectionFactory.Create();
        var pendingOperation = Task.Run(() => operation(connection), CancellationToken.None);

        try
        {
            return await pendingOperation.WaitAsync(timeout, cancellationToken);
        }
        catch (TimeoutException exception)
        {
            connection.Dispose();
            throw new LdapProtocolException(LdapFailureCodes.Timeout, exception);
        }
        catch (OperationCanceledException)
        {
            connection.Dispose();
            throw;
        }
        catch (LdapException exception) when (
            isCredentialBind && exception.ErrorCode == InvalidCredentialsErrorCode)
        {
            throw new LdapProtocolException(LdapFailureCodes.InvalidCredentials, exception);
        }
        catch (Exception exception) when (ContainsTlsFailure(exception))
        {
            throw new LdapProtocolException(LdapFailureCodes.TlsValidationFailed, exception);
        }
        catch (LdapProtocolException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new LdapProtocolException(LdapFailureCodes.Unavailable, exception);
        }
    }

    private async Task<IReadOnlyList<LdapDirectoryEntry>> SearchAsync(
        string filter,
        CancellationToken cancellationToken)
    {
        var searchBase = string.IsNullOrWhiteSpace(ldapConfig.UserSearchBase)
            ? ldapConfig.LdapDomainName
            : ldapConfig.UserSearchBase;
        var requestedAttributes = GetRequestedAttributes();

        return await ExecuteAsync(
            connection =>
            {
                BindSearchConnection(connection);
                var request = new SearchRequest(
                    searchBase,
                    filter,
                    SearchScope.Subtree,
                    requestedAttributes)
                {
                    TimeLimit = TimeSpan.FromSeconds(ldapConfig.SearchTimeoutSeconds)
                };
                var response = (SearchResponse)connection.SendRequest(
                    request,
                    TimeSpan.FromSeconds(ldapConfig.SearchTimeoutSeconds));
                return MapEntries(response);
            },
            TimeSpan.FromSeconds(
                Math.Max(ldapConfig.ConnectTimeoutSeconds, ldapConfig.SearchTimeoutSeconds)),
            false,
            cancellationToken);
    }

    private void BindSearchConnection(LdapConnection connection)
    {
        PrepareTransport(connection);
        if (string.IsNullOrWhiteSpace(ldapConfig.BindDn))
        {
            connection.AuthType = AuthType.Anonymous;
            connection.Bind();
            return;
        }

        connection.AuthType = AuthType.Basic;
        connection.Bind(new NetworkCredential(ldapConfig.BindDn, ldapConfig.BindPassword));
    }

    private void PrepareTransport(LdapConnection connection)
    {
        if (ldapConfig.UseStartTls)
            connection.SessionOptions.StartTransportLayerSecurity(null);
    }

    private string[] GetRequestedAttributes()
    {
        var attributes = ldapConfig.Attributes;
        return new[]
            {
                attributes.ImmutableId,
                attributes.EmployeeId,
                attributes.UserPrincipalName,
                attributes.Email,
                attributes.DisplayName,
                attributes.Department,
                attributes.AccountStatus
            }
            .Where(attribute => !string.IsNullOrWhiteSpace(attribute))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IReadOnlyList<LdapDirectoryEntry> MapEntries(SearchResponse response)
    {
        var results = new List<LdapDirectoryEntry>(response.Entries.Count);
        foreach (SearchResultEntry entry in response.Entries)
        {
            var attributes =
                new Dictionary<string, IReadOnlyList<object>>(StringComparer.OrdinalIgnoreCase);
            foreach (string attributeName in entry.Attributes.AttributeNames)
            {
                var attribute = entry.Attributes[attributeName];
                var values = new List<object>(attribute.Count);
                for (var index = 0; index < attribute.Count; index++)
                    values.Add(attribute[index]);
                attributes[attributeName] = values;
            }

            results.Add(new LdapDirectoryEntry
            {
                DistinguishedName = entry.DistinguishedName,
                Attributes = attributes
            });
        }

        return results;
    }

    private static bool ContainsTlsFailure(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
            if (current is AuthenticationException)
                return true;

        return false;
    }
}
