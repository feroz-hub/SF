using System.Linq.Expressions;
using DomainValidation.Interfaces.Specification;
using DomainValidation.Validation;
using Zentra.Domain.Constants.Endpoint;
using Zentra.Domain.Models.Endpoint.Validation;
using Zentra.Service.Implementation.Endpoint.Extensions;

namespace Zentra.Service.Implementation.Endpoint.Validators;

public class IsRequestNull<TModel> : ISpecification<TModel>
    where TModel : ValidatedBaseModel
{
    private readonly Expression<Func<TModel, object>> expression;

    public IsRequestNull(Expression<Func<TModel, object>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        var type = value.GetType();
        var returnValue = false;
        if (type == typeof(string))
            returnValue = !string.IsNullOrWhiteSpace(value as string);
        else if (type == typeof(int))
            returnValue = (int)value <= 0;
        else if (type == typeof(DateTime)) returnValue = ((DateTime)value).Equals(DateTime.MinValue);

        return returnValue;
    }
}

public class IsRequestValidUri<TModel> : ISpecification<TModel>
    where TModel : ValidatedBaseModel
{
    private readonly Expression<Func<TModel, string>> expression;

    public IsRequestValidUri(Expression<Func<TModel, string>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        var type = value.GetType();
        if (type.Equals(typeof(string))) return value.IsValidUrl();

        return false;
    }
}

public class IsRequestValidUris<TModel> : ISpecification<TModel>
{
    private readonly Expression<Func<TModel, List<string>>> expression;

    public IsRequestValidUris(Expression<Func<TModel, List<string>>> expression)
    {
        this.expression = expression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var value = expression.Compile()(entity);
        var type = value.GetType();
        var returnValue = false;
        if (type != typeof(List<string>)) return false;

        foreach (var valueString in value)
        {
            returnValue = valueString.IsValidUrl();
            if (!returnValue) return false;
        }

        return returnValue;
    }
}

public class CheckLengthRestrictions<TModel> : ISpecification<TModel>
{
    private readonly Expression<Func<TModel, string>> compareExpression;
    private readonly Expression<Func<TModel, string>> sourceExpression;
    private readonly Expression<Func<TModel, int>> targetExpression;

    public CheckLengthRestrictions(
        Expression<Func<TModel, string>> sourceExpression,
        Expression<Func<TModel, int>> targetExpression,
        Expression<Func<TModel, string>> compareExpression)
    {
        this.sourceExpression = sourceExpression;
        this.targetExpression = targetExpression;
        this.compareExpression = compareExpression;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        var sourceValue = sourceExpression.Compile()(entity);
        var targetValue = targetExpression.Compile()(entity);
        var compareValue = compareExpression.Compile()(entity);
        var sourceType = sourceValue.GetType();
        var targetType = targetValue.GetType();
        var compareType = compareValue.GetType();
        var id = string.Empty;
        var compare = string.Empty;
        var lengthRestrictions = 0;

        if (sourceType == typeof(string)) id = sourceValue;

        if (targetType == typeof(int)) lengthRestrictions = targetValue;

        if (compareType == typeof(string)) compare = compareValue;

        return compare switch
        {
            ">" when id.Length > lengthRestrictions => false,
            ">" => true,
            "<" when id.Length < lengthRestrictions => false,
            "<" => true,
            _ => false
        };
    }
}

public class CheckClientAuthorizedForGrantType<TModel> : ISpecification<TModel>
    where TModel : ValidatedBaseModel
{
    private readonly List<string> grantTypes;

    public CheckClientAuthorizedForGrantType(List<string> grantTypes)
    {
        this.grantTypes = grantTypes;
    }

    public bool IsSatisfiedBy(TModel entity)
    {
        foreach (var grantType in grantTypes)
            if (entity.Client.SupportedGrantTypes.ToList().Contains(grantType))
                return true;

        return false;
    }
}

public class BaseRequestModelValidator<TModel> : Validator<TModel>
    where TModel : ValidatedBaseModel
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
}

public class CheckRequestedScopes<TModel> : ISpecification<TModel>
    where TModel : ValidatedBaseModel
{
    public bool IsSatisfiedBy(TModel entity)
    {
        var scopes = entity.GetValue(OpenIdConstants.TokenRequest.Scope);
        if (string.IsNullOrWhiteSpace(scopes))
        {
            if (entity.Client.AllowedScopes.ContainsAny())
            {
                scopes = string.Join(" ", entity.Client.AllowedScopes.ToArray());
                entity.RequestRawData["scope"] = scopes;
            }
            else
            {
                return false;
            }
        }

        if (string.IsNullOrWhiteSpace(scopes)) return false;

        return true;
    }
}
