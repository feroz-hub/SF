/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Domain.Enums;
using HCL.CS.Service.Implementation.Api.Services;
using Xunit;

namespace HCL.CS.UnitTests;

public sealed class LocalAuthenticationPolicyTests
{
    [Fact]
    public void Resolve_LdapExplicitlyDisabled_ReturnsLocal()
    {
        var configuration = CreateCompleteLdapConfiguration();
        configuration.SystemSettings.LdapConfig.Enabled = false;

        new AuthenticationModeResolver(configuration).Resolve().Should().Be(AuthenticationMode.Local);
    }

    [Theory]
    [InlineData("host")]
    [InlineData("base")]
    [InlineData("filter")]
    [InlineData("immutable")]
    [InlineData("email")]
    [InlineData("display")]
    [InlineData("status")]
    public void Resolve_LdapEnabledButMandatorySettingMissing_ReturnsLocal(string missing)
    {
        var configuration = CreateCompleteLdapConfiguration();
        switch (missing)
        {
            case "host":
                configuration.SystemSettings.LdapConfig.LdapHostName = "";
                break;
            case "base":
                configuration.SystemSettings.LdapConfig.LdapDomainName = "";
                break;
            case "filter":
                configuration.SystemSettings.LdapConfig.UserSearchFilter = "";
                break;
            case "immutable":
                configuration.SystemSettings.LdapConfig.Attributes.ImmutableId = "";
                break;
            case "email":
                configuration.SystemSettings.LdapConfig.Attributes.Email = "";
                break;
            case "display":
                configuration.SystemSettings.LdapConfig.Attributes.DisplayName = "";
                break;
            case "status":
                configuration.SystemSettings.LdapConfig.Attributes.AccountStatus = "";
                break;
        }

        new AuthenticationModeResolver(configuration).Resolve().Should().Be(AuthenticationMode.Local);
    }

    [Fact]
    public void Resolve_LdapEnabledAndComplete_ReturnsLdap()
    {
        new AuthenticationModeResolver(CreateCompleteLdapConfiguration())
            .Resolve()
            .Should()
            .Be(AuthenticationMode.Ldap);
    }

    [Fact]
    public void Resolve_RuntimeAvailabilityCannotChangeConfiguredMode()
    {
        var resolver = new AuthenticationModeResolver(CreateCompleteLdapConfiguration());

        resolver.Resolve().Should().Be(AuthenticationMode.Ldap);
        resolver.Resolve().Should().Be(AuthenticationMode.Ldap);
    }

    [Theory]
    [InlineData("ferozebasha.s@hcltech.com", "ferozebasha.s@hcltech.com")]
    [InlineData("john.doe@hcltech.com", "john.doe@hcltech.com")]
    [InlineData("USER123@HCLTECH.COM", "user123@hcltech.com")]
    [InlineData("  USER123@HCLTECH.COM  ", "user123@hcltech.com")]
    public void Validate_ExactCorporateDomain_IsAccepted(string input, string normalized)
    {
        var result = CreateDomainPolicy().Validate(input);

        result.IsValid.Should().BeTrue();
        result.NormalizedEmail.Should().Be(normalized);
        result.NormalizedDomain.Should().Be("hcltech.com");
        result.ReasonCode.Should().BeNull();
    }

    [Theory]
    [InlineData("ferozebasha@gmail.com")]
    [InlineData("user@hcl.com")]
    [InlineData("user@hcltech.co")]
    [InlineData("user@sub.hcltech.com")]
    [InlineData("user@hcltech.com.example.com")]
    [InlineData("user@fakehcltech.com")]
    [InlineData("user+hcltech.com@gmail.com")]
    public void Validate_NonExactCorporateDomain_IsRejected(string input)
    {
        var result = CreateDomainPolicy().Validate(input);

        result.IsValid.Should().BeFalse();
        result.ReasonCode.Should().Be("AUTH_LOCAL_EMAIL_DOMAIN_NOT_ALLOWED");
    }

    [Theory]
    [InlineData("hcltech.com")]
    [InlineData("@hcltech.com")]
    [InlineData("user@")]
    [InlineData("")]
    [InlineData("Display Name <user@hcltech.com>")]
    [InlineData("user@hcltech.com,attacker@example.com")]
    [InlineData("user@hcltech\u3002com")]
    public void Validate_MalformedOrUnicodeDomain_IsRejected(string input)
    {
        var result = CreateDomainPolicy().Validate(input);

        result.IsValid.Should().BeFalse();
        result.ReasonCode.Should().Be("AUTH_LOCAL_EMAIL_INVALID");
    }

    [Fact]
    public void ConfigurationValidator_RequiresExactCorporateDomain()
    {
        var configuration = CreateCompleteLdapConfiguration()
            .SystemSettings.LocalAuthenticationConfig;
        configuration.AllowedEmailDomains = ["*.hcltech.com"];

        var errors = new LocalAuthenticationConfigurationValidator().Validate(configuration);

        errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AvailabilityModel_DoesNotExposeInfrastructureConfiguration()
    {
        var availability = HCL.CS.Domain.Models.Api.AuthenticationAvailabilityModel.Create(
            AuthenticationMode.Local,
            true);

        availability.AuthenticationMode.Should().Be("LOCAL");
        availability.LocalRegistrationAvailable.Should().BeTrue();
        availability.AllowedEmailDomainHint.Should().Be("@hcltech.com");
        availability.GetType().GetProperties().Should().NotContain(property =>
            property.Name.Contains("Host", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("Bind", StringComparison.OrdinalIgnoreCase)
            || property.Name.Contains("Smtp", StringComparison.OrdinalIgnoreCase));
    }

    private static AllowedEmailDomainPolicy CreateDomainPolicy()
    {
        return new AllowedEmailDomainPolicy(CreateCompleteLdapConfiguration());
    }

    private static HclCsConfig CreateCompleteLdapConfiguration()
    {
        var configuration = new HclCsConfig();
        var ldap = configuration.SystemSettings.LdapConfig;
        ldap.Enabled = true;
        ldap.LdapHostName = "ldap.example.internal";
        ldap.LdapPort = 636;
        ldap.LdapDomainName = "DC=example,DC=internal";
        ldap.UserSearchBase = "OU=Users,DC=example,DC=internal";
        ldap.UserSearchFilter = "(userPrincipalName={0})";
        ldap.UseSsl = true;
        ldap.UseStartTls = false;
        return configuration;
    }
}
