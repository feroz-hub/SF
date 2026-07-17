using System;
using System.Threading.Tasks;
using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Service.Interfaces;
using HCL.CS.TestApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HCL.CS.IntegrationTests.Api
{
    public class AuthenticationServiceTests : HclCsFakeSetup
    {
        private IAuthenticationService authenticationService;
        private IUserAccountService userAccountService;

        public AuthenticationServiceTests()
        {
            userAccountService = ServiceProvider.GetService<IUserAccountService>();
            authenticationService = ServiceProvider.GetService<IAuthenticationService>();
        }

        [Fact]
        public async Task Login()
        {
            //var result = await authenticationService.PasswordSignInAsync("", "");

            //result = await authenticationService.PasswordSignInAsync("adminuser", "");

            //result = await authenticationService.PasswordSignInAsync("adminUshyjger", "Test@123");

            //result = await authenticationService.PasswordSignInAsync("adminUser", "Test@123456");

            //var result = await authenticationService.PasswordSignInAsync("Jesu", "Test@123");

            // Success
            var result = await authenticationService.PasswordSignInAsync("adminUser", "Test@123");
        }

        [Fact]
        public async Task TwoFactorSMS()
        {
            var result = await authenticationService.TwoFactorEmailSignInAsync("Test@123");
        }

        [Fact]
        public async Task SignOutAsync()
        {
            await authenticationService.SignOutAsync();
        }

        [Fact]
        public async Task SetupAuthenticatorAppAsync_Tests()
        {
            var usermodel = UserHelper.CreateModel();
            usermodel.UserName = "TestingTestingdfsdf";
            usermodel.Email = "testingsdfsdf@test.com";
            var securityQuestion = await userAccountService.GetAllSecurityQuestionsAsync();
            if (securityQuestion != null && securityQuestion.Count > 0)
            {
                usermodel.UserSecurityQuestion[0].SecurityQuestionId = securityQuestion[0].Id;
            }

            FrameworkResult framResult = await userAccountService.RegisterUserAsync(usermodel);

            var result = await authenticationService.SetupAuthenticatorAppAsync(new Guid("763C134B-B796-41F5-B22E-08D9C46BAFAF"), "HCL.CS");

            framResult.Should().BeOfType<FrameworkResult>();
            framResult.Status.Should().Be(ResultStatus.Success);
        }
    }
}



