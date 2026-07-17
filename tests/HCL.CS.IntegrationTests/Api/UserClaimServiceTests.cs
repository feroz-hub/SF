using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HCL.CS.Domain;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models;
using HCL.CS.Service.Interfaces;
using HCL.CS.TestApp.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Xunit;

namespace HCL.CS.IntegrationTests.Api
{
   public class UserClaimServiceTests : HclCsFakeSetup
    {
        private readonly IUserAccountService userAccountService;
        private readonly IAuditTrailService auditTrailService;

        public UserClaimServiceTests()
        {
            userAccountService = ServiceProvider.GetService<IUserAccountService>();
            auditTrailService = ServiceProvider.GetService<IAuditTrailService>();
        }


        [Fact]
        [Trait("Category", "SuccessCase")]
        public async Task AddClaimAsync_Success()
        {
            Random random = new Random();
            UserClaimModel userClaimModel = UserHelper.CreateUserClaimModel();
            Guid userid = new Guid("1026C140-2813-4FB8-A20D-E45834987AD7");
            userClaimModel.ClaimType = userClaimModel.ClaimType + string.Empty + random.Next();
            userClaimModel.UserId = userid;
            FrameworkResult result = await userAccountService.AddClaimAsync(userClaimModel);
            result.Should().BeOfType<FrameworkResult>();
            result.Status.Should().Be(ResultStatus.Success);

            // Audit
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1);

            PagingModel page = new PagingModel()
            {
                CurrentPage = 1,
                ItemsPerPage = 1000,
            };

            AuditResponseModel auditResponseModelResult = await auditTrailService.GetAuditDetailsAsync(userClaimModel.CreatedBy, fromDate, toDate, page);
            auditResponseModelResult.Should().NotBeNull();

            var resultaudit = auditResponseModelResult.AuditList.Find(i => i.NewValue.Contains(userClaimModel.ClaimType));
            resultaudit.Should().NotBeNull();
        }




        [Fact]
        [Trait("Category", "SuccessCase")]
        public async Task AddUserClaimAsync_SuccessCase()
        {
            Random random = new Random();
            UserClaimModel userClaimModel = UserHelper.CreateUserClaimModel();
            Guid userid = new Guid("1026C140-2813-4FB8-A24D-E40834986AD7");
            userClaimModel.ClaimType = userClaimModel.ClaimType + string.Empty + random.Next();
            userClaimModel.UserId = userid;
            FrameworkResult result = await userAccountService.AddClaimAsync(userClaimModel);
            result.Should().BeOfType<FrameworkResult>();
            result.Status.Should().Be(ResultStatus.Success);
        }


        [Fact]
        [Trait("Category", "SuccessCase")]
        public async Task AddClaimAsyncbyList_Success()
        {
            Guid userid = new Guid("1026C140-2813-4FB8-A24D-E40834986AD7");
            IList< UserClaimModel> userClaimModel = UserHelper.CreateUserClaimModel_List(userid);
            FrameworkResult result = await userAccountService.AddClaimAsync(userClaimModel);
            result.Should().BeOfType<FrameworkResult>();
            result.Status.Should().Be(ResultStatus.Success);


            // Audit
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1);

            PagingModel page = new PagingModel()
            {
                CurrentPage = 1,
                ItemsPerPage = 1000,
            };

            AuditResponseModel auditResponseModelResult = await auditTrailService.GetAuditDetailsAsync(userClaimModel[0].CreatedBy, fromDate, toDate, page);
            auditResponseModelResult.Should().NotBeNull();
            var resultaudit = auditResponseModelResult.AuditList.Find(i => i.NewValue.Contains(userClaimModel[0].ClaimType));
            resultaudit.Should().NotBeNull();

        }



        [Fact]
        [Trait("Category", "SuccessCase")]
        public async Task RemoveClaimAsync_Success()
        {
            Guid userid = new Guid("1026C140-2813-4FB8-A24D-E40834986AD7");
            IList<UserClaimModel> userClaimModel = UserHelper.CreateUserClaimModel_List(userid);
            FrameworkResult result = await userAccountService.RemoveClaimAsync(userClaimModel);
            result.Should().BeOfType<FrameworkResult>();
            result.Status.Should().Be(ResultStatus.Success);


            // Audit
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1);

            PagingModel page = new PagingModel()
            {
                CurrentPage = 1,
                ItemsPerPage = 1000,
            };

            AuditResponseModel auditResponseModelResult = await auditTrailService.GetAuditDetailsAsync(userClaimModel[0].CreatedBy, fromDate, toDate, page);
            auditResponseModelResult.Should().NotBeNull();
        }

        [Fact]
        [Trait("Category", "SuccessCase")]
        public async Task RemoveClaimAsyncbymodle_Success()
        {
            Random random = new Random();
            UserClaimModel userClaimModel = UserHelper.CreateUserClaimModel();
            Guid userid = new Guid("1026C140-2813-4FB8-A24D-E40834986AD7");
            userClaimModel.ClaimType = userClaimModel.ClaimType + string.Empty + random.Next();
            userClaimModel.UserId = userid;
            FrameworkResult result = await userAccountService.AddClaimAsync(userClaimModel);
            result.Should().BeOfType<FrameworkResult>();
            result.Status.Should().Be(ResultStatus.Success);

            // Remove Claim
            FrameworkResult result1 = await userAccountService.RemoveClaimAsync(userClaimModel);
            result1.Should().BeOfType<FrameworkResult>();
            result1.Status.Should().Be(ResultStatus.Success);

            // Audit
            DateTime fromDate = DateTime.Today;
            DateTime toDate = DateTime.Today.AddDays(1);

            PagingModel page = new PagingModel()
            {
                CurrentPage = 1,
                ItemsPerPage = 1000,
            };

            AuditResponseModel auditResponseModelResult = await auditTrailService.GetAuditDetailsAsync(userClaimModel.CreatedBy, fromDate, toDate, page);
            auditResponseModelResult.Should().NotBeNull();

            var resultaudit = auditResponseModelResult.AuditList.Find(i => i.NewValue.Contains(userClaimModel.ClaimType));
            resultaudit.Should().NotBeNull();
        }

    }
}



