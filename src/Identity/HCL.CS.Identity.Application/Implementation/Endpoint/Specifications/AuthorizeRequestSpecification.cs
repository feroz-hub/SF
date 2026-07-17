using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AutoMapper;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Domain.Entities.Endpoint;
using HCL.CS.Domain.ErrorCodes;
using HCL.CS.Domain.Models.Endpoint;
using HCL.CS.Domain.Models.Endpoint.Request;
using HCL.CS.DomainServices;
using HCL.CS.Service.Implementation.Endpoint.Extensions;
using HCL.CS.Service.Implementation.Endpoint.Validators;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint;
using HCL.CS.Service.Interfaces.Interfaces.Endpoint.Validators;
using static HCL.CS.Domain.Constants.Endpoint.AuthenticationConstants;

namespace HCL.CS.Service.Implementation.Endpoint.Specifications;

internal sealed class AuthorizeRequestSpecification : BaseRequestModelValidator<ValidatedAuthorizeRequestModel>
{
    internal AuthorizeRequestSpecification(
        IResourceScopeValidator resourceScopeValidator,
        IRepository<Clients> clientRepository,
        IMapper mapper,
        ISessionManagementService session,
        TokenSettings configSettings)
    {
        Add("ValidateJwtRequestUri", new Rule<ValidatedAuthorizeRequestModel>(
            new ValidateJwtRequest(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.JwtRequestUriNotSupported));

        Add("ValidateRequestedClient", new Rule<ValidatedAuthorizeRequestModel>(
            new ValidateRequestedClientId(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.ClientIdMissingInRequest));

        Add("ValidateRequestedClientLength", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckLengthRestrictions<ValidatedAuthorizeRequestModel>(
                request => request.GetValue(OpenIdConstants.AuthorizeRequest.ClientId),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.ClientId,
                request => ">"),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.ClientIdTooLong));

        Add("ValidateUriNull", new Rule<ValidatedAuthorizeRequestModel>(
            new IsRequestNull<ValidatedAuthorizeRequestModel>(request =>
                request.GetValue(OpenIdConstants.AuthorizeRequest.RedirectUri)), OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidRedirectUri));

        Add("ValidateUriLength", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckLengthRestrictions<ValidatedAuthorizeRequestModel>(
                request => request.GetValue(OpenIdConstants.AuthorizeRequest.RedirectUri),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.RedirectUri,
                request => ">"),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.RedirectUriTooLong));

        Add("ValidateUriFormat", new Rule<ValidatedAuthorizeRequestModel>(
            new IsRequestValidUri<ValidatedAuthorizeRequestModel>(request =>
                request.GetValue(OpenIdConstants.AuthorizeRequest.RedirectUri)),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidRedirectUri));

        Add("CheckValidClientFromDB", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckValidClient(clientRepository, mapper),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.ClientDoesNotExist));

        Add("CheckConfidentialClientSigningAlgorithm", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckConfidentialClientSigningAlgorithm(),
            OpenIdConstants.Errors.UnauthorizedClient,
            EndpointErrorCodes.SigningAlgorithmIsInvalid));

        Add("CheckResponseType", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckResponseType(),
            OpenIdConstants.Errors.UnsupportedResponseType,
            EndpointErrorCodes.ResponseTypeMissing));

        Add("CheckClientRedirectUriForAuthCodeFlow", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckClientRedirectUriForAuthCodeFlow(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.RedirectURIIsMandatory));

        Add("CheckValidClientRedirectUris", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckClientRedirectUri(request => request.Client.RedirectUris.ConvertAll(uri => uri.RedirectUri)),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.RedirectUriNotRegistered));

        ValidatePkceParameters();
        ValidateResponseMode();
        ValidateScopes(resourceScopeValidator);
        ValidateCoreParameters(session, configSettings);
    }

    public void ValidatePkceParameters()
    {
        Add("ValidatePkce", new Rule<ValidatedAuthorizeRequestModel>(
            new ValidatePkce(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidCodeChallenge));
    }

    private void ValidateResponseMode()
    {
        Add(
            "CheckResponseMode", new Rule<ValidatedAuthorizeRequestModel>(
                new CheckResponseMode(),
                OpenIdConstants.Errors.UnsupportedResponseType,
                EndpointErrorCodes.InvalidResponseMode));

        Add("CheckAllowedGrantTypeForClient", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckAllowedGrantTypeForClient(),
            OpenIdConstants.Errors.UnsupportedGrantType,
            EndpointErrorCodes.InvalidGrantTypeForClient));

        Add("CheckAccessTokenInResponseType", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckAccessTokenInResponseType(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.ClientNotConfiguredToReceiveAccessToken));
    }

    private void ValidateCoreParameters(ISessionManagementService session, TokenSettings configSettings)
    {
        Add("CheckState", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckState(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidState));

        Add("CheckNonce", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckNonce(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidNonce));

        Add("CheckPrompt", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckPrompt(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidPrompt));

        Add("CheckMaxAge", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckMaxAge(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.InvalidMaxAge));
    }

    private void ValidateScopes(IResourceScopeValidator resourceScopeValidator)
    {
        Add("CheckRequestedScopes", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckRequestedScopes<ValidatedAuthorizeRequestModel>(),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.InvalidScopeOrNotAllowed));

        Add("CheckScopeLengthRestrictions", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckLengthRestrictions<ValidatedAuthorizeRequestModel>(
                request => request.GetValue(OpenIdConstants.AuthorizeRequest.Scope),
                request => request.TokenConfigOptions.InputLengthRestrictionsConfig.Scope,
                request => ">"),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.ScopeTooLong));

        Add("CheckScopeResponseTypePlausibility", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckScopeResponseTypePlausibility(),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.ResponseTypeRequiresOpenIdScope));

        Add("CheckScopesAllowedForClient", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckScopesAllowedForClient(resourceScopeValidator),
            OpenIdConstants.Errors.InvalidRequest,
            EndpointErrorCodes.RequestedScopeNotAllowedForClient));

        Add("CheckOpenId", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckOpenId(),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.OpenIdScopeMissing));

        Add("CheckScopeRequirementIdentity", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckScopeRequirementIdentity(),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.MustIncludeIdentityScopes));

        Add("CheckOfflineAccessForCodeIdToken", new Rule<ValidatedAuthorizeRequestModel>(
            new CheckOfflineAccessForCodeIdToken(),
            OpenIdConstants.Errors.InvalidScope,
            EndpointErrorCodes.MustNotIncludeOfflineAccessScope));
    }
}

internal class ValidateJwtRequest : ISpecification<ValidatedAuthorizeRequestModel>
{
    // TODO: Check with team on removing this validation and related attributes.

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var jwtRequest = model.GetValue(OpenIdConstants.AuthorizeRequest.Request);
        var jwtRequestUri = model.GetValue(OpenIdConstants.AuthorizeRequest.RequestUri);
        return string.IsNullOrWhiteSpace(jwtRequest) || string.IsNullOrWhiteSpace(jwtRequestUri);
    }
}

internal class ValidateRequestedClientId : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var clientId = model.GetValue(OpenIdConstants.AuthorizeRequest.ClientId);
        if (string.IsNullOrWhiteSpace(clientId)) return false;

        model.ClientId = clientId;
        return true;
    }
}

internal class CheckValidClient : ISpecification<ValidatedAuthorizeRequestModel>
{
    private readonly IRepository<Clients> clientRepository;
    private readonly IMapper mapper;

    internal CheckValidClient(
        IRepository<Clients> clientRepository,
        IMapper mapper)
    {
        this.clientRepository = clientRepository;
        this.mapper = mapper;
    }

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var clientId = model.GetValue(OpenIdConstants.AuthorizeRequest.ClientId);

        var clientsEntity = clientRepository.GetAsync(
            client => client.ClientId == clientId,
            new System.Linq.Expressions.Expression<Func<HCL.CS.Domain.Entities.Endpoint.Clients, object>>[] { x => x.RedirectUris, x => x.PostLogoutRedirectUris }).GetAwaiter().GetResult();

        if (!clientsEntity.ContainsAny()) return false;

        model.Client = mapper.Map<Clients, ClientsModel>(clientsEntity[0]);
        return true;
    }
}

internal class CheckConfidentialClientSigningAlgorithm : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (model.Client == null) return false;

        if (string.IsNullOrWhiteSpace(model.Client.AllowedSigningAlgorithm)) return true;

        return string.Equals(model.Client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.RsaSha256,
                   StringComparison.Ordinal)
               || string.Equals(model.Client.AllowedSigningAlgorithm, OpenIdConstants.Algorithms.EcdsaSha256,
                   StringComparison.Ordinal);
    }
}

internal class CheckClientRedirectUri : ISpecification<ValidatedAuthorizeRequestModel>
{
    private readonly Expression<Func<ValidatedAuthorizeRequestModel, List<string>>> expression;

    internal CheckClientRedirectUri(Expression<Func<ValidatedAuthorizeRequestModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var value = expression.Compile()(model);

        if (value == null) return false;

        var type = value.GetType();
        if (!string.IsNullOrWhiteSpace(model.RedirectUri))
        {
            if (type == typeof(List<string>) &&
                value.Any(uri => string.Equals(uri, model.RedirectUri, StringComparison.Ordinal))) return true;

            return false;
        }

        return true;
    }
}

internal class CheckClientRedirectUriForAuthCodeFlow : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        return model.GrantType != GrantType.AuthorizationCode
               || !string.IsNullOrWhiteSpace(model.RedirectUri);
    }
}

// TODO: Check and remove this spec[CheckAllowedForAuthorizeEndpoint] below if its not required. Currently its not referred anywhere.

internal class CheckAllowedForAuthorizeEndpoint : ISpecification<ValidatedAuthorizeRequestModel>
{
    private readonly Expression<Func<ValidatedAuthorizeRequestModel, string>> expression;

    internal CheckAllowedForAuthorizeEndpoint(Expression<Func<ValidatedAuthorizeRequestModel, string>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var value = expression.Compile()(model);
        var type = value.GetType();
        return type == typeof(string) && AllowedGrantTypesForAuthorizeEndpoint.Contains(value);
    }
}

internal class ValidatePkce : ISpecification<ValidatedAuthorizeRequestModel>
{
    private static readonly Regex CodeChallengePattern =
        new("^[A-Za-z0-9_-]+$", RegexOptions.CultureInvariant | RegexOptions.Compiled);

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (model.GrantType == GrantType.AuthorizationCode)
        {
            var codeChallenge = model.GetValue(OpenIdConstants.AuthorizeRequest.CodeChallenge);
            var codeChallengeMethod = model.GetValue(OpenIdConstants.AuthorizeRequest.CodeChallengeMethod);

            var isPublicClient = !model.Client.RequireClientSecret;
            if (model.Client.RequirePkce || isPublicClient || !string.IsNullOrWhiteSpace(codeChallenge))
            {
                if (string.IsNullOrWhiteSpace(codeChallenge)) return false;

                if (codeChallenge.Length <
                    model.TokenConfigOptions.InputLengthRestrictionsConfig.CodeChallengeMinLength ||
                    codeChallenge.Length >
                    model.TokenConfigOptions.InputLengthRestrictionsConfig.CodeChallengeMaxLength)
                    return false;

                if (!CodeChallengePattern.IsMatch(codeChallenge)) return false;

                model.CodeChallenge = codeChallenge;

                if (string.IsNullOrWhiteSpace(codeChallengeMethod)) return false;

                if (!codeChallengeMethod.Equals(OpenIdConstants.CodeChallengeMethods.Sha256, StringComparison.Ordinal))
                    return false;

                model.CodeChallengeMethod = codeChallengeMethod;
            }
        }

        return true;
    }
}

internal class CheckResponseType : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var responseType = model.GetValue(OpenIdConstants.AuthorizeRequest.ResponseType);
        var state = model.GetValue(OpenIdConstants.AuthorizeRequest.State);

        if (!string.IsNullOrWhiteSpace(state)) model.State = state;

        if (string.IsNullOrWhiteSpace(responseType)) return false;

        // check response type is in supported list
        if (!AllowedResponseTypes.Contains(responseType)) return false;

        if (!AllowedGrantTypeForResponseType.Keys.Contains(responseType)) return false;

        model.ResponseType = responseType;
// model.GrantType =
        if (AllowedGrantTypesForAuthorizeEndpoint.Contains(AllowedGrantTypeForResponseType[model.ResponseType]))
        {
            model.GrantType = AllowedGrantTypeForResponseType[model.ResponseType];
            model.ResponseMode = AllowedResponseModesForGrantType[model.GrantType].First();
            model.RedirectUri = model.GetValue(OpenIdConstants.AuthorizeRequest.RedirectUri);
        }
        else
        {
            return false;
        }

        return true;
    }
}

internal class CheckAllowedGrantTypeForEndpoint : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (AllowedGrantTypesForAuthorizeEndpoint.Contains(model.GrantType)) return true;

        return false;
    }
}

internal class CheckResponseMode : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var responseMode = model.GetValue(OpenIdConstants.AuthorizeRequest.ResponseMode);
        if (!string.IsNullOrWhiteSpace(responseMode))
        {
            if (!AllowedResponseModes.Contains(responseMode)) return false;

            if (!AllowedResponseModesForGrantType[model.GrantType].Contains(responseMode)) return false;

            model.ResponseMode = responseMode;
        }

        return true;
    }
}

internal class CheckAllowedGrantTypeForClient : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (!model.Client.SupportedGrantTypes.Contains(model.GrantType)) return false;

        return true;
    }
}

internal class CheckAccessTokenInResponseType : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var responseTypes = model.GetValue(OpenIdConstants.AuthorizeRequest.ResponseType).SplitBySpace();
        return !responseTypes.Contains(OpenIdConstants.ResponseTypes.Token) ||
               model.Client.AllowAccessTokensViaBrowser;
    }
}

internal class CheckNonce : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var nonce = model.GetValue(OpenIdConstants.AuthorizeRequest.Nonce);
        if (string.IsNullOrWhiteSpace(nonce)) return !model.IsOpenIdRequest;

        if (nonce.Length > model.TokenConfigOptions.InputLengthRestrictionsConfig.Nonce) return false;

        model.Nonce = nonce;
        return true;
    }
}

internal class CheckState : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var state = model.GetValue(OpenIdConstants.AuthorizeRequest.State);
        if (string.IsNullOrWhiteSpace(state)) return false;

        if (state.Length > model.TokenConfigOptions.InputLengthRestrictionsConfig.State) return false;

        model.State = state;
        return true;
    }
}

internal class CheckPrompt : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var prompt = model.GetValue(OpenIdConstants.AuthorizeRequest.Prompt);
        if (string.IsNullOrWhiteSpace(prompt)) return true;

        var prompts = prompt.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (prompts.All(x => AllowedPromptModes.Contains(x)))
        {
            if (prompts.Contains(OpenIdConstants.PromptModes.None) && prompts.Length > 1) return false;

            model.PromptModes = prompts;
            return true;
        }

        return false;
    }
}

internal class CheckMaxAge : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var maxAge = model.GetValue(OpenIdConstants.AuthorizeRequest.MaxAge);
        if (!string.IsNullOrWhiteSpace(maxAge))
        {
            if (!int.TryParse(maxAge, out var seconds)) return false;

            if (seconds >= 0)
            {
                model.MaxAge = seconds;
                return true;
            }
        }

        return true;
    }
}

internal class CheckScopeResponseTypePlausibility : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var scope = model.GetValue(OpenIdConstants.AuthorizeRequest.Scope);
        if (scope != null)
        {
            model.RequestedScopes = scope.ParseScopesString().ToList();
            if (model.RequestedScopes.Contains(IdentityScopes.OpenId)) model.IsOpenIdRequest = true;
        }

        // check scope vs response_type plausability
        var requirement = ResponseTypeToScope[model.ResponseType];
        if (requirement == ScopeRequirement.Identity ||
            requirement == ScopeRequirement.IdentityOnly)
            return model.IsOpenIdRequest;

        return true;
    }
}

internal class CheckScopesAllowedForClient : ISpecification<ValidatedAuthorizeRequestModel>
{
    private readonly IResourceScopeValidator resourceScopeValidator;

    internal CheckScopesAllowedForClient(IResourceScopeValidator resourceScopeValidator)
    {
        this.resourceScopeValidator = resourceScopeValidator;
    }

    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        var checkClientScope = resourceScopeValidator
            .ValidateRequestedScopeWithClientAsync(model.Client.AllowedScopes, model.RequestedScopes)
            .GetAwaiter().GetResult();
        if (!checkClientScope) return false;

        var resourceScopeModel = new ResourceScopeModel
        {
            RawData = model.RequestRawData,
            RequestedScope = model.RequestedScopes,
            Client = model.Client,
            UserName = model.Subject is { Identity: not null } ? model.Subject.Identity.Name : string.Empty
        };

        model.AllowedScopesParserModel = resourceScopeValidator.ValidateRequestedScopesAsync(resourceScopeModel)
            .GetAwaiter().GetResult();

        return true;
    }
}

internal class CheckOpenId : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        // TODO Error boolean in AllowedScopesParserModel
        //if (model.AllowedScopesParserModel.IsError)
        //{
        //    return false;
        //}

        if (model.AllowedScopesParserModel.ParsedIdentityResources.ContainsAny() && !model.IsOpenIdRequest)
            return false;

        if (model.AllowedScopesParserModel.ParsedApiScopes.ContainsAny()) model.IsApiResourceRequest = true;

        return true;
    }
}

internal class CheckScopeRequirementIdentity : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (ResponseTypeToScope[model.ResponseType] == ScopeRequirement.Identity
            && !model.AllowedScopesParserModel.ParsedIdentityResources.ContainsAny())
            return false;

        return true;
    }
}

internal class CheckOfflineAccessForCodeIdToken : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (model.ResponseType == OpenIdConstants.ResponseTypes.CodeIdToken &&
            model.AllowedScopesParserModel.AllowOfflineAccess) return false;

        return true;
    }
}

internal class CheckScopeRequirementIdentityOnly : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (ResponseTypeToScope[model.ResponseType] == ScopeRequirement.IdentityOnly
            && (!model.AllowedScopesParserModel.ParsedIdentityResources.ContainsAny() ||
                !model.AllowedScopesParserModel.ParsedApiScopes.ContainsAny()))
            return false;

        return true;
    }
}

internal class CheckScopeRequirementResourceOnly : ISpecification<ValidatedAuthorizeRequestModel>
{
    public bool IsSatisfiedBy(ValidatedAuthorizeRequestModel model)
    {
        if (ResponseTypeToScope[model.ResponseType] == ScopeRequirement.ResourceOnly
            && (model.AllowedScopesParserModel.ParsedIdentityResources.ContainsAny() ||
                !model.AllowedScopesParserModel.ParsedApiScopes.ContainsAny()))
            return false;

        return true;
    }
}
