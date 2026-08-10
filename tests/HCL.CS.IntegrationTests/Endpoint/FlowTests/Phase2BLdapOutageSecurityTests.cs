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
using HCL.CS.Domain.Models.Api;
using HCL.CS.Service.Interfaces.Interfaces.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace IntegrationTests.Endpoint.FlowTests;

public sealed class Phase2BLdapOutageSecurityTests : HclCsFakeSetup
{
    private readonly UnavailableLdapAuthenticationService unavailableLdap = new();

    public Phase2BLdapOutageSecurityTests()
    {
        OnPostConfigureServices += services =>
        {
            var descriptor = services.Last(service => service.ServiceType == typeof(HclCsConfig));
            var configuration = (HclCsConfig)descriptor.ImplementationInstance!;
            var ldap = configuration.SystemSettings.LdapConfig;
            ldap.Enabled = true;
            ldap.LdapHostName = "ldap.example.internal";
            ldap.LdapPort = 636;
            ldap.LdapDomainName = "DC=example,DC=internal";
            ldap.UserSearchBase = "OU=Users,DC=example,DC=internal";
            ldap.UserSearchFilter = "(userPrincipalName={0})";
            ldap.UseSsl = true;
            ldap.UseStartTls = false;

            services.RemoveAll<ILdapAuthenticationService>();
            services.AddSingleton<ILdapAuthenticationService>(unavailableLdap);
        };
        Initialize();
    }

    [Fact]
    [Trait("Category", "Phase2B")]
    public async Task ConfiguredLdapOutage_DoesNotAttemptLocalPasswordOrEnableRegistration()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var resolver = scope.ServiceProvider.GetRequiredService<IAuthenticationModeResolver>();
        var authentication = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
        var accounts = scope.ServiceProvider.GetRequiredService<IUserAccountService>();

        resolver.Resolve().Should().Be(AuthenticationMode.Ldap);
        var availability = await authentication.GetAuthenticationAvailabilityAsync();
        availability.AuthenticationMode.Should().Be("LDAP");
        availability.LocalRegistrationAvailable.Should().BeFalse();

        var login = await authentication.PasswordSignInAsync("checktest", "Test@123456789");
        login.Succeeded.Should().BeFalse();
        login.ErrorCode.Should().Be("AUTH_DIRECTORY_UNAVAILABLE");
        unavailableLdap.CallCount.Should().Be(1);

        var registration = await accounts.RegisterUserAsync(new UserModel
        {
            UserName = "OutageUser",
            Email = "outage.user@hcltech.com",
            Password = "LocalUser@12345",
            FirstName = "Outage",
            LastName = "User",
            UserSecurityQuestion = [],
            UserClaims = []
        });
        registration.Status.Should().Be(ResultStatus.Failed);
        registration.Errors.Single().Code.Should().Be("AUTH_MODE_LOCAL_REGISTRATION_DISABLED");
    }

    private sealed class UnavailableLdapAuthenticationService : ILdapAuthenticationService
    {
        public int CallCount { get; private set; }

        public Task<LdapAuthenticationResult> AuthenticateAsync(
            string username,
            string password,
            CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(LdapAuthenticationResult.Failed("LDAP_UNAVAILABLE"));
        }
    }
}
