/*
- Copyright (c) 2021 HCL CORPORATION.
- All rights reserved. HCL source code is an unpublished work and the use of a copyright notice does not imply otherwise.
- This source code contains confidential, trade secret material of HCL. Any attempt or participation in deciphering,
- decoding, reverse engineering or in any way altering the source code is strictly prohibited, unless the prior written consent of
- HCL is obtained. This is proprietary and confidential to HCL.
 */

using System.Security.Claims;
using AutoMapper;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Api;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Api;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices.Infra;
using HCL.CS.DomainServices.Repository.Api;
using HCL.CS.DomainServices.Wrappers;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;

namespace HCL.CS.Service.Implementation.Endpoint.Services;

internal class UserInfoServices : SecurityBase, IUserInfoServices
{
    private readonly IFrameworkResultService frameworkResultService;
    private readonly IIdentityResourceRepository identityResourceRepository;
    private readonly ILoggerService loggerService;
    private readonly IMapper mapper;
    private readonly IUserClaimRepository userClaimRepository;
    private readonly UserManagerWrapper<Users> userManager;

    public UserInfoServices(
        ILoggerInstance instance,
        IIdentityResourceRepository identityResourceRepository,
        UserManagerWrapper<Users> userManager,
        IFrameworkResultService frameworkResultService,
        IMapper mapper,
        IUserClaimRepository userClaimRepository)
    {
        this.identityResourceRepository = identityResourceRepository;
        this.userManager = userManager;
        this.frameworkResultService = frameworkResultService;
        this.mapper = mapper;
        this.userClaimRepository = userClaimRepository;
        loggerService = instance.GetLoggerInstance(LoggerKeyConstants.DefaultLoggerKey);
    }

    public async Task<Dictionary<string, object>> ProcessUserInfoAsync(
        ValidatedUserInfoRequestModel userInfoRequestValidation)
    {
        try
        {
            // Get Scopes.
            var scopes = userInfoRequestValidation.Claims.Where(c => c.Type == OpenIdConstants.ClaimTypes.Scope)
                .Select(c => c.Value).ToList();
            if (!scopes.ContainsAny()) frameworkResultService.Throw(EndpointErrorCodes.InvalidScopeClaims);

            var identityResources = await identityResourceRepository.GetAllIdentityResourcesByScopesAsync(scopes);
            if (userInfoRequestValidation.Subject != null &&
                userInfoRequestValidation.Subject.Identity is ClaimsIdentity)
            {
                loggerService.WriteTo(Log.Debug,
                    "Entered into process user information for user : " + userInfoRequestValidation.Subject);
                var claimIdentity = userInfoRequestValidation.Subject.Identity as ClaimsIdentity;
                var claim = claimIdentity.FindFirst(OpenIdConstants.ClaimTypes.Sub);
                if (claim != null && claim.Value.IsGuid())
                {
                    var claimUser = await GetByIdAsync(claim.Value);
                    var allowedClaims = await claimUser.GetUserIdentityResources(identityResources);

                    var rolesOfUser = await userManager.GetRolesAsync(mapper.Map<Users>(claimUser));

                    if (rolesOfUser.ContainsAny())
                        foreach (var role in rolesOfUser)
                            allowedClaims.Add(new Claim(ClaimTypes.Role, role));

                    // add sub claim.
                    var subClaim = allowedClaims.SingleOrDefault(x => x.Type == OpenIdConstants.ClaimTypes.Sub);
                    if (subClaim == null)
                        allowedClaims.Add(new Claim(OpenIdConstants.ClaimTypes.Sub, claim.Value));
                    else if (subClaim.Value != claim.Value)
                        frameworkResultService.Throw(EndpointErrorCodes.SubjectClaimValueMismatch);

                    return allowedClaims.Distinct().ToList().ConvertCollection();
                }
            }

            loggerService.WriteToWithCaller(Log.Error, "Invalid or No claim present");
            frameworkResultService.Throw(EndpointErrorCodes.ArgumentNullError);
            return null;
        }
        catch (Exception ex)
        {
            loggerService.WriteToWithCaller(Log.Error, ex, ex.Message);
            throw;
        }
    }

    private async Task<UserModel> GetByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var userModel = mapper.Map<Users, UserModel>(user);
            var userClaims = await userClaimRepository.GetClaimsAsync(user.Id);
            if (userClaims.ContainsAny())
            {
                var userClaimModel = mapper.Map<IList<UserClaims>, IList<UserClaimModel>>(userClaims);
                userModel.UserClaims = (List<UserClaimModel>)userClaimModel;
            }

            return userModel;
        }

        return null;
    }
}
