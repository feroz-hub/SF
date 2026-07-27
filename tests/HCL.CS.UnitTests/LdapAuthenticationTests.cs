/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Reflection;
using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Api;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Implementation.Api.Ldap;
using HCL.CS.Service.Implementation.Api.Services;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using Xunit;

namespace HCL.CS.UnitTests;

public class LdapAuthenticationTests
{
    [Fact]
    public async Task AuthenticateAsync_ValidUser_ReturnsNormalizedProfile()
    {
        var directoryId = Guid.Parse("8fc28e21-7a79-4e33-a25b-0cf753c93250");
        var protocol = new FakeLdapProtocolClient
        {
            Entries = [CreateEntry(directoryId, "  Alice@Example.COM ", " Alice Example ", "512")]
        };
        var audit = new RecordingAuditLogger();
        var service = CreateService(protocol, audit);

        var result = await service.AuthenticateAsync("Alice@Example.COM", "correct-password");

        result.IsAuthenticated.Should().BeTrue();
        result.FailureCode.Should().BeNull();
        result.User.Should().NotBeNull();
        result.User!.ImmutableId.Should().Be(directoryId.ToString("D"));
        result.User.EmployeeId.Should().Be("E12345");
        result.User.Email.Should().Be("alice@example.com");
        result.User.DisplayName.Should().Be("Alice Example");
        result.User.UserPrincipalName.Should().Be("alice@example.com");
        result.User.Department.Should().Be("Security");
        result.User.AccountStatus.Should().Be("ACTIVE");
        audit.Events.Should().ContainSingle(eventValue =>
            eventValue.StartsWith("success:", StringComparison.Ordinal));
        audit.Events.Should().NotContain(eventValue => eventValue.Contains("correct-password"));
    }

    [Fact]
    public async Task AuthenticateAsync_EscapesLdapFilterInput()
    {
        var protocol = new FakeLdapProtocolClient { Entries = [] };
        var service = CreateService(protocol);

        await service.AuthenticateAsync(@"a*)(uid=*)\b" + '\0', "password");

        protocol.EscapedIdentifier.Should().Be(@"a\2a\29\28uid=\2a\29\5cb\00");
    }

    [Theory]
    [InlineData(0, "LDAP_USER_NOT_FOUND")]
    [InlineData(2, "LDAP_MULTIPLE_USERS_FOUND")]
    public async Task AuthenticateAsync_InvalidSearchCardinality_ReturnsGenericInternalFailure(
        int count,
        string expectedFailure)
    {
        var entries = Enumerable.Range(0, count)
            .Select(index => CreateEntry(Guid.NewGuid(), $"user{index}@example.com", "Test User", "512"))
            .ToArray();
        var service = CreateService(new FakeLdapProtocolClient { Entries = entries });

        var result = await service.AuthenticateAsync("test@example.com", "password");

        result.IsAuthenticated.Should().BeFalse();
        result.FailureCode.Should().Be(expectedFailure);
        result.User.Should().BeNull();
    }

    [Theory]
    [InlineData("514", "LDAP_ACCOUNT_DISABLED")]
    [InlineData("528", "LDAP_ACCOUNT_LOCKED")]
    [InlineData("8389120", "LDAP_ACCOUNT_EXPIRED")]
    [InlineData("malformed", "LDAP_SCHEMA_MAPPING_FAILED")]
    public async Task AuthenticateAsync_InactiveOrMalformedStatus_IsRejected(
        string status,
        string expectedFailure)
    {
        var protocol = new FakeLdapProtocolClient
        {
            Entries = [CreateEntry(Guid.NewGuid(), "user@example.com", "Test User", status)]
        };
        var service = CreateService(protocol);

        var result = await service.AuthenticateAsync("user@example.com", "password");

        result.IsAuthenticated.Should().BeFalse();
        result.FailureCode.Should().Be(expectedFailure);
    }

    [Theory]
    [InlineData("objectGUID")]
    [InlineData("mail")]
    [InlineData("displayName")]
    public void AttributeMapper_MissingRequiredAttribute_IsRejected(string attributeName)
    {
        var entry = CreateEntry(Guid.NewGuid(), "user@example.com", "Test User", "512");
        var attributes = entry.Attributes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value,
            StringComparer.OrdinalIgnoreCase);
        attributes.Remove(attributeName);
        var mapper = CreateMapper(CreateConfiguration());

        var action = () => mapper.Map(new LdapDirectoryEntry
        {
            DistinguishedName = entry.DistinguishedName,
            Attributes = attributes
        });

        action.Should().Throw<LdapProfileValidationException>()
            .Which.FailureCode.Should().Be("LDAP_REQUIRED_ATTRIBUTE_MISSING");
    }

    [Fact]
    public void AttributeMapper_InvalidEmail_IsRejected()
    {
        var mapper = CreateMapper(CreateConfiguration());

        var action = () => mapper.Map(
            CreateEntry(Guid.NewGuid(), "not-an-email", "Test User", "512"));

        action.Should().Throw<LdapProfileValidationException>()
            .Which.FailureCode.Should().Be("LDAP_EMAIL_INVALID");
    }

    [Fact]
    public void AttributeMapper_MalformedBinaryImmutableId_IsRejected()
    {
        var mapper = CreateMapper(CreateConfiguration());
        var entry = CreateEntry(Guid.NewGuid(), "user@example.com", "Test User", "512");
        var attributes = entry.Attributes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value,
            StringComparer.OrdinalIgnoreCase);
        attributes["objectGUID"] = [new byte[] { 1, 2, 3 }];

        var action = () => mapper.Map(new LdapDirectoryEntry
        {
            DistinguishedName = entry.DistinguishedName,
            Attributes = attributes
        });

        action.Should().Throw<LdapProfileValidationException>()
            .Which.FailureCode.Should().Be("LDAP_IMMUTABLE_ID_INVALID");
    }

    [Theory]
    [InlineData(true, false, "employeeID")]
    [InlineData(false, true, "department")]
    public void AttributeMapper_MissingConfiguredMandatoryAttribute_IsRejected(
        bool requireEmployeeId,
        bool requireDepartment,
        string attributeName)
    {
        var configuration = CreateConfiguration();
        configuration.SystemSettings.LdapConfig.RequireEmployeeId = requireEmployeeId;
        configuration.SystemSettings.LdapConfig.RequireDepartment = requireDepartment;
        var mapper = CreateMapper(configuration);
        var entry = CreateEntry(Guid.NewGuid(), "user@example.com", "Test User", "512");
        var attributes = entry.Attributes.ToDictionary(
            pair => pair.Key,
            pair => pair.Value,
            StringComparer.OrdinalIgnoreCase);
        attributes.Remove(attributeName);

        var action = () => mapper.Map(new LdapDirectoryEntry
        {
            DistinguishedName = entry.DistinguishedName,
            Attributes = attributes
        });

        action.Should().Throw<LdapProfileValidationException>()
            .Which.FailureCode.Should().Be("LDAP_REQUIRED_ATTRIBUTE_MISSING");
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidCredentials_DoesNotExposePassword()
    {
        const string password = "NeverLogThisPassword!";
        var protocol = new FakeLdapProtocolClient
        {
            Entries = [CreateEntry(Guid.NewGuid(), "user@example.com", "Test User", "512")],
            CredentialFailure = new LdapProtocolException(LdapFailureCodes.InvalidCredentials)
        };
        var audit = new RecordingAuditLogger();
        var service = CreateService(protocol, audit);

        var result = await service.AuthenticateAsync("user@example.com", password);

        result.IsAuthenticated.Should().BeFalse();
        result.FailureCode.Should().Be("LDAP_INVALID_CREDENTIALS");
        result.GetType().GetProperties().Should().NotContain(property =>
            property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase));
        result.User.Should().BeNull();
        audit.Events.Should().NotContain(eventValue => eventValue.Contains(password));
    }

    [Theory]
    [InlineData("LDAP_TIMEOUT")]
    [InlineData("LDAP_UNAVAILABLE")]
    [InlineData("LDAP_TLS_VALIDATION_FAILED")]
    public async Task AuthenticateAsync_ProtocolFailure_IsSafelyMapped(string failureCode)
    {
        var protocol = new FakeLdapProtocolClient
        {
            SearchFailure = new LdapProtocolException(failureCode)
        };
        var service = CreateService(protocol);

        var result = await service.AuthenticateAsync("user@example.com", "password");

        result.IsAuthenticated.Should().BeFalse();
        result.FailureCode.Should().Be(failureCode);
    }

    [Fact]
    public async Task AuthenticateAsync_Cancellation_IsPropagated()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var service = CreateService(new FakeLdapProtocolClient());

        var action = () => service.AuthenticateAsync(
            "user@example.com",
            "password",
            cancellation.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public void ConfigurationValidator_RejectsUnencryptedNonLoopbackTransport()
    {
        var configuration = CreateConfiguration().SystemSettings.LdapConfig;
        configuration.UseSsl = false;
        configuration.UseStartTls = false;
        configuration.AllowUnencryptedForDevelopment = true;
        configuration.LdapHostName = "ldap.example.internal";

        var errors = new LdapConfigurationValidator().Validate(configuration);

        errors.Should().Contain("Encrypted LDAP transport is required.");
    }

    [Fact]
    public void ConfigurationValidator_RejectsSslAndStartTlsTogether()
    {
        var configuration = CreateConfiguration().SystemSettings.LdapConfig;
        configuration.UseSsl = true;
        configuration.UseStartTls = true;

        var errors = new LdapConfigurationValidator().Validate(configuration);

        errors.Should().Contain("LDAP SSL and StartTLS cannot both be enabled.");
    }

    [Fact]
    public async Task AuthenticateAsync_DisabledConfiguration_DoesNotContactDirectory()
    {
        var configuration = CreateConfiguration();
        configuration.SystemSettings.LdapConfig.Enabled = false;
        var protocol = new FakeLdapProtocolClient();
        var service = new LdapAuthenticationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            CreateMapper(configuration),
            new RecordingAuditLogger());

        var result = await service.AuthenticateAsync("user@example.com", "password");

        result.IsAuthenticated.Should().BeFalse();
        result.FailureCode.Should().Be("LDAP_CONFIGURATION_INVALID");
        protocol.EscapedIdentifier.Should().BeNull();
    }

    [Fact]
    public void AccountStatusEvaluator_MapsSupportedStates()
    {
        var evaluator = new LdapAccountStatusEvaluator();

        evaluator.Evaluate("512").Should().Be(LdapAccountState.Active);
        evaluator.Evaluate("514").Should().Be(LdapAccountState.Disabled);
        evaluator.Evaluate("528").Should().Be(LdapAccountState.Locked);
        evaluator.Evaluate("8389120").Should().Be(LdapAccountState.Expired);
        evaluator.Evaluate(null).Should().Be(LdapAccountState.Unknown);
    }

    [Fact]
    public void LdapShadowCredentialMarker_IsFixedAndContainsNoDirectoryPassword()
    {
        var marker = typeof(UserAccountService)
            .GetField(
                "ExternalCredentialOnlyPasswordHash",
                BindingFlags.NonPublic | BindingFlags.Static)?
            .GetRawConstantValue() as string;

        marker.Should().Be("!LDAP_EXTERNAL_CREDENTIAL_ONLY!");
        marker!.Contains("password", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task AccountValidation_ActiveImmutableIdentity_SucceedsWithoutUserPassword()
    {
        var immutableId = Guid.NewGuid();
        var configuration = CreateConfiguration();
        var protocol = new FakeLdapProtocolClient
        {
            Entries = [CreateEntry(immutableId, "user@example.com", "Test User", "512")]
        };
        var service = new LdapAccountValidationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            CreateMapper(configuration));

        var result = await service.ValidateAccountAsync(immutableId.ToString("D"));

        result.IsActive.Should().BeTrue();
        result.AccountState.Should().Be(LdapAccountState.Active);
        protocol.CredentialValidationCount.Should().Be(0);
    }

    [Theory]
    [InlineData("514", LdapAccountState.Disabled, "LDAP_ACCOUNT_DISABLED")]
    [InlineData("528", LdapAccountState.Locked, "LDAP_ACCOUNT_LOCKED")]
    [InlineData("8389120", LdapAccountState.Expired, "LDAP_ACCOUNT_EXPIRED")]
    public async Task AccountValidation_InactiveState_FailsClosed(
        string rawStatus,
        LdapAccountState expectedState,
        string expectedCode)
    {
        var immutableId = Guid.NewGuid();
        var configuration = CreateConfiguration();
        var protocol = new FakeLdapProtocolClient
        {
            Entries = [CreateEntry(immutableId, "user@example.com", "Test User", rawStatus)]
        };
        var service = new LdapAccountValidationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            CreateMapper(configuration));

        var result = await service.ValidateAccountAsync(immutableId.ToString("D"));

        result.IsActive.Should().BeFalse();
        result.AccountState.Should().Be(expectedState);
        result.FailureCode.Should().Be(expectedCode);
    }

    [Theory]
    [InlineData(0, "LDAP_USER_NOT_FOUND")]
    [InlineData(2, "LDAP_MULTIPLE_USERS_FOUND")]
    public async Task AccountValidation_SearchCardinality_FailsClosed(int resultCount, string expectedCode)
    {
        var immutableId = Guid.NewGuid();
        var configuration = CreateConfiguration();
        var protocol = new FakeLdapProtocolClient
        {
            Entries = Enumerable.Range(0, resultCount)
                .Select(_ => CreateEntry(immutableId, "user@example.com", "Test User", "512"))
                .ToArray()
        };
        var service = new LdapAccountValidationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            CreateMapper(configuration));

        var result = await service.ValidateAccountAsync(immutableId.ToString("D"));

        result.IsActive.Should().BeFalse();
        result.FailureCode.Should().Be(expectedCode);
    }

    [Fact]
    public async Task AccountValidation_DirectoryUnavailable_FailsClosed()
    {
        var configuration = CreateConfiguration();
        var protocol = new FakeLdapProtocolClient
        {
            SearchFailure = new LdapProtocolException(LdapFailureCodes.Unavailable)
        };
        var service = new LdapAccountValidationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            CreateMapper(configuration));

        var result = await service.ValidateAccountAsync(Guid.NewGuid().ToString("D"));

        result.IsActive.Should().BeFalse();
        result.FailureCode.Should().Be(LdapFailureCodes.Unavailable);
    }

    [Fact]
    public void EscapeBytes_ProducesRfc4515BinaryAssertionValue()
    {
        LdapFilterEncoder.EscapeBytes(new byte[] { 0x00, 0x2a, 0xff })
            .Should().Be(@"\00\2a\ff");
    }

    private static LdapAuthenticationService CreateService(
        FakeLdapProtocolClient protocol,
        RecordingAuditLogger? audit = null)
    {
        var configuration = CreateConfiguration();
        var evaluator = new LdapAccountStatusEvaluator();
        return new LdapAuthenticationService(
            configuration,
            new LdapConfigurationValidator(),
            protocol,
            new LdapAttributeMapper(configuration, evaluator),
            audit ?? new RecordingAuditLogger());
    }

    private static LdapAttributeMapper CreateMapper(HclCsConfig configuration)
    {
        return new LdapAttributeMapper(configuration, new LdapAccountStatusEvaluator());
    }

    private static HclCsConfig CreateConfiguration()
    {
        return new HclCsConfig
        {
            SystemSettings = new SystemSettings
            {
                LdapConfig = new LdapConfig
                {
                    Enabled = true,
                    LdapHostName = "ldap.example.internal",
                    LdapPort = 636,
                    UseSsl = true,
                    UseStartTls = false,
                    LdapDomainName = "DC=example,DC=internal",
                    UserSearchBase = "OU=Users,DC=example,DC=internal",
                    UserSearchFilter = "(userPrincipalName={0})",
                    ConnectTimeoutSeconds = 10,
                    SearchTimeoutSeconds = 10,
                    Attributes = new LdapAttributeConfig()
                }
            }
        };
    }

    private static LdapDirectoryEntry CreateEntry(
        Guid immutableId,
        string email,
        string displayName,
        string accountStatus)
    {
        return new LdapDirectoryEntry
        {
            DistinguishedName = "CN=Test User,OU=Users,DC=example,DC=internal",
            Attributes = new Dictionary<string, IReadOnlyList<object>>(StringComparer.OrdinalIgnoreCase)
            {
                ["objectGUID"] = [immutableId.ToByteArray()],
                ["employeeID"] = ["E12345"],
                ["userPrincipalName"] = [email],
                ["mail"] = [email],
                ["displayName"] = [displayName],
                ["department"] = ["Security"],
                ["userAccountControl"] = [accountStatus]
            }
        };
    }

    private sealed class FakeLdapProtocolClient : ILdapProtocolClient
    {
        public IReadOnlyList<LdapDirectoryEntry> Entries { get; init; } = [];

        public Exception? SearchFailure { get; init; }

        public Exception? CredentialFailure { get; init; }

        public string? EscapedIdentifier { get; private set; }

        public int CredentialValidationCount { get; private set; }

        public Task<IReadOnlyList<LdapDirectoryEntry>> SearchUsersAsync(
            string escapedIdentifier,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EscapedIdentifier = escapedIdentifier;
            if (SearchFailure is not null) throw SearchFailure;
            return Task.FromResult(Entries);
        }

        public Task ValidateCredentialsAsync(
            string distinguishedName,
            string password,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            CredentialValidationCount++;
            if (CredentialFailure is not null) throw CredentialFailure;
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<LdapDirectoryEntry>> SearchByImmutableIdAsync(
            string directoryImmutableId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (SearchFailure is not null) throw SearchFailure;
            return Task.FromResult(Entries);
        }
    }

    private sealed class RecordingAuditLogger : ILdapAuthenticationAuditLogger
    {
        public List<string> Events { get; } = [];

        public void AuthenticationSucceeded(string pseudonymousUserIdentifier)
        {
            Events.Add("success:" + pseudonymousUserIdentifier);
        }

        public void AuthenticationFailed(string failureCode, string pseudonymousUserIdentifier)
        {
            Events.Add("failure:" + failureCode + ":" + pseudonymousUserIdentifier);
        }
    }
}
