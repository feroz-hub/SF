using System.Linq.Expressions;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using HCL.CS.Domain;
using HCL.CS.Domain.Configurations.Endpoint;
using HCL.CS.Domain.Constants;
using HCL.CS.Domain.Constants.Endpoint;
using HCL.CS.Service.Extension;
using HCL.CS.Service.Implementation.Endpoint.Extensions;

namespace HCL.CS.Service.Implementation.Api.Validators;

public class CheckTokenExpirationRange<TModel>(
    Expression<Func<TModel, object>> expression,
    string tokenType,
    TokenExpiration tokenExpiration)
    : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    public bool IsSatisfiedBy(TModel? entity)
    {
        if (entity == null) return false;

        var value = expression.Compile()(entity);
        if (value == null) return false;
        var type = value.GetType();
        if (!(type == typeof(int))) return false;
        var tokenValue = (int)value;

        return tokenType switch
        {
            OpenIdConstants.TokenType.AccessToken => tokenValue >= tokenExpiration.MinAccessTokenExpiration &&
                                                     tokenValue <= tokenExpiration.MaxAccessTokenExpiration,
            OpenIdConstants.TokenType.IdentityToken => tokenValue >= tokenExpiration.MinIdentityTokenExpiration &&
                                                       tokenValue <= tokenExpiration.MaxIdentityTokenExpiration,
            OpenIdConstants.TokenType.RefreshToken => tokenValue >= tokenExpiration.MinRefreshTokenExpiration &&
                                                      tokenValue <= tokenExpiration.MaxRefreshTokenExpiration,
            OpenIdConstants.TokenType.AuthorizationCode =>
                tokenValue >= tokenExpiration.MinAuthorizationCodeExpiration &&
                tokenValue <= tokenExpiration.MaxAuthorizationCodeExpiration,
            OpenIdConstants.TokenType.LogoutToken => tokenValue >= tokenExpiration.MinLogoutTokenExpiration &&
                                                     tokenValue <= tokenExpiration.MaxLogoutTokenExpiration,
            _ => false
        };
    }
}

public class IsNotNull<TModel>(Expression<Func<TModel, object>> expression) : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    public bool IsSatisfiedBy(TModel? entity)
    {
        if (entity == null) return false;

        var value = expression.Compile()(entity);
        if (value == null) return false;
        var type = value.GetType();
        if (type == typeof(string)) return !string.IsNullOrWhiteSpace(value as string);

        if (type == typeof(int)) return (int)value > 0;

        if (type == typeof(DateTime)) return !((DateTime)value).Equals(DateTime.MinValue);

        if (type == typeof(Guid)) return !((Guid)value).Equals(Guid.Empty);

        return value != null;
    }
}

public class AreNotNull<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, List<string>>> expression;

    public AreNotNull(Expression<Func<TModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (value == null) return false;

        var type = value.GetType();
        if (type.Equals(typeof(List<string>)))
            foreach (var valueString in value)
                if (string.IsNullOrWhiteSpace(valueString))
                    return false;

        return true;
    }
}

public class IsValidIdentifier<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, object>> expression;

    public IsValidIdentifier(Expression<Func<TModel, object>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        if (entity == null) return false;

        var value = expression.Compile()(entity);
        if (value != null)
        {
            var type = value.GetType();
            if (type.Equals(typeof(Guid))) return ((Guid)value).IsValid();

            if (type.Equals(typeof(int))) return (int)value > 0;
        }

        return value != null;
    }
}

public class IsValidIdentifierExists<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, List<Guid>>> expression;

    public IsValidIdentifierExists(Expression<Func<TModel, List<Guid>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (value.ContainsAny())
        {
            var type = value.GetType();
            if (type.Equals(typeof(List<Guid>)) && value.ContainsAny())
                foreach (var valueString in value)
                    if (!valueString.IsValid())
                        return false;
        }

        return true;
    }
}

public class IsValidUris<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, List<string>>> expression;

    public IsValidUris(Expression<Func<TModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (value.ContainsAny())
        {
            var type = value.GetType();
            if (type.Equals(typeof(List<string>)))
                foreach (var valueString in value)
                    if (!valueString.IsValidUrl())
                        return false;
        }

        return true;
    }
}

public class IsValidUri<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, string>> expression;

    public IsValidUri(Expression<Func<TModel, string>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (!string.IsNullOrWhiteSpace(value))
        {
            var type = value.GetType();
            if (type.Equals(typeof(string))) return value.IsValidUrl();
        }

        return false;
    }
}

public class IsValid255CharLength<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, string>> expression;

    public IsValid255CharLength(Expression<Func<TModel, string>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (!string.IsNullOrWhiteSpace(value))
        {
            var type = value.GetType();
            if (type.Equals(typeof(string)) && value.Length > Constants.ColumnLength255) return false;
        }

        return true;
    }
}

public class IsValid255CharLengths<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, List<string>>> expression;

    public IsValid255CharLengths(Expression<Func<TModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (value.ContainsAny())
        {
            var type = value.GetType();
            if (type.Equals(typeof(List<string>)))
                foreach (var valueString in value)
                    if (!string.IsNullOrWhiteSpace(valueString) && valueString.Length > Constants.ColumnLength255)
                        return false;
        }

        return true;
    }
}

public class IsValid2048CharLengths<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, List<string>>> expression;

    public IsValid2048CharLengths(Expression<Func<TModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        if (value.ContainsAny())
        {
            var type = value.GetType();
            if (type.Equals(typeof(List<string>)))
                foreach (var valueString in value)
                    if (!string.IsNullOrWhiteSpace(valueString) && valueString.Length > Constants.ColumnLength2048)
                        return false;
        }

        return true;
    }
}

public class BaseDomainModelValidator<TModel> : Validator<TModel>
    where TModel : BaseTrailModel
{
    public bool IsValid { get; set; }

    public async Task<ValidationError> ValidateAsync(TModel model)
    {
        var validationError = new ValidationError();
        var validationResult = Validate(model);
        if (!validationResult.IsValid)
        {
            IsValid = false;
            var error = validationResult.Errors.FirstOrDefault();
            if (error == null) return validationError;

            validationError.ErrorCode = error.ErrorCode;
            validationError.ErrorMessage = error.ErrorMessage;
        }
        else
        {
            IsValid = true;
        }

        return await Task.FromResult(validationError);
    }

    public async Task<List<FrameworkError>> ValidateAsync(IList<TModel> models)
    {
        var validationErrors = new List<FrameworkError>();
        foreach (var model in models)
        {
            var validationResult = ValidateAll(model);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors;
                if (!errors.ContainsAny()) return validationErrors;

                foreach (var error in errors)
                {
                    var validationError = new FrameworkError
                    {
                        Code = error.ErrorCode,
                        Description = error.ErrorMessage
                    };
                    validationErrors.Add(validationError);
                }
            }
        }

        IsValid = !validationErrors.ContainsAny();
        return await Task.FromResult(validationErrors);
    }
}

public class CheckAlgorithm<TModel> : ISpecification<TModel>
    where TModel : BaseTrailModel
{
    private readonly Expression<Func<TModel, object>> expression;

    public CheckAlgorithm(Expression<Func<TModel, object>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);

        var type = value.GetType();
        if (type.Equals(typeof(string)))
        {
            if (string.IsNullOrWhiteSpace(value.ToString())) return false;

            var allowedSigningAlgorithms = new[]
            {
                OpenIdConstants.Algorithms.RsaSha256,
                OpenIdConstants.Algorithms.EcdsaSha256
            };
            if (!allowedSigningAlgorithms.Contains(value.ToString(), StringComparer.Ordinal)) return false;
        }

        return true;
    }
}
