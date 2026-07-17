using System.Linq.Expressions;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.ErrorCodes;
using Zentra.Domain.Models.Endpoint;
using Zentra.Domain.Models.Endpoint.Request;
using Zentra.Service.Implementation.Endpoint.Extensions;
using Zentra.Service.Implementation.Endpoint.Validators;
using Zentra.Service.Interfaces.Interfaces.Endpoint.Validators;
using static Zentra.Domain.Constants.Endpoint.AuthenticationConstants;

namespace Zentra.Service.Implementation.Endpoint.Specifications;

internal sealed class ClientCredentialsFlowSpecification : BaseRequestModelValidator<ValidatedTokenRequestModel>
{
    internal ClientCredentialsFlowSpecification(IResourceScopeValidator resourceScopeValidator)
    {
        Add("CheckClientAuthorizedForGrantType", new Rule<ValidatedTokenRequestModel>(
            new CheckClientAuthorizedForGrantType<ValidatedTokenRequestModel>(new List<string>
            {
                GrantType.ClientCredentials
            }),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.ClientNotAuthorizedForGrantType));
        Add("ValidateRequestedScopes", new Rule<ValidatedTokenRequestModel>(
            new ValidateRequestedClientCredentialScopes(resourceScopeValidator),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.InvalidScopeOrNotAllowed));
        Add("CheckParsedIdentityResources", new Rule<ValidatedTokenRequestModel>(
            new CheckParsedIdentityResources(request => request.AllowedScopesParserModel),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.OpenIdScopeNotAllowed));
        Add("CheckOfflineAccess", new Rule<ValidatedTokenRequestModel>(
            new CheckOfflineAccess(request => request.AllowedScopesParserModel),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.RefreshTokenRequestNotAllowed));
    }
}

internal class ValidateRequestedClientCredentialScopes : ISpecification<ValidatedTokenRequestModel>
{
    private readonly IResourceScopeValidator resourceScopeValidator;

    internal ValidateRequestedClientCredentialScopes(IResourceScopeValidator resourceScopeValidator)
    {
        this.resourceScopeValidator = resourceScopeValidator;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var scopes = model.GetValue(OpenIdConstants.TokenRequest.Scope);

        if (string.IsNullOrWhiteSpace(scopes))
        {
            if (model.Client.AllowedScopes.ContainsAny())
            {
                var allowedScopes = model.Client.AllowedScopes.Except(typeof(IdentityScopes).GetArray().ToList());
                scopes = string.Join(" ", allowedScopes.ToArray());
            }
            else
            {
                return false;
            }
        }
        else
        {
            // To check any identity scope present in client credential if yes return false to throw invalid scope error.
            var scopeList = scopes.SplitBySpace().Select(x => x.ToLower());
            var identityList = typeof(IdentityScopes).GetArray().ToList();
            if (scopeList.Intersect(identityList).ContainsAny()) return false;
        }

        if (scopes.Length > model.TokenConfigOptions.InputLengthRestrictionsConfig.Scope) return false;

        var requestedScopes = scopes.ParseScopesString().ToList();
        var checkClientScope = resourceScopeValidator
            .ValidateRequestedScopeWithClientAsync(model.Client.AllowedScopes, requestedScopes).GetAwaiter()
            .GetResult();
        AllowedScopesParserModel allowedScopesParser;
        if (checkClientScope)
        {
            var resourceScopeModel = new ResourceScopeModel
            {
                RawData = model.RequestRawData,
                RequestedScope = requestedScopes,
                Client = model.Client
            };

            allowedScopesParser = resourceScopeValidator.ValidateRequestedScopesAsync(resourceScopeModel).GetAwaiter()
                .GetResult();
            model.TokenDetails = allowedScopesParser.TokenDetails;
        }
        else
        {
            return false;
        }

        model.AllowedScopesParserModel = allowedScopesParser;

        return true;
    }
}

internal class CheckParsedIdentityResources : ISpecification<ValidatedTokenRequestModel>
{
    private readonly Expression<Func<ValidatedTokenRequestModel, object>> expression;

    internal CheckParsedIdentityResources(Expression<Func<ValidatedTokenRequestModel, object>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var value = expression.Compile()(model);
        if (value == null) return false;

        var type = value.GetType();
        if (type != typeof(AllowedScopesParserModel)) return false;

        if (model.AllowedScopesParserModel.ParsedIdentityResources.ContainsAny()) return false;

        return true;
    }
}

internal class CheckOfflineAccess : ISpecification<ValidatedTokenRequestModel>
{
    private readonly Expression<Func<ValidatedTokenRequestModel, object>> expression;

    internal CheckOfflineAccess(Expression<Func<ValidatedTokenRequestModel, object>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(ValidatedTokenRequestModel model)
    {
        var value = expression.Compile()(model);
        if (value == null) return false;

        var type = value.GetType();
        if (type != typeof(AllowedScopesParserModel)) return false;

        if (model.AllowedScopesParserModel.AllowOfflineAccess) return false;

        return true;
    }
}
