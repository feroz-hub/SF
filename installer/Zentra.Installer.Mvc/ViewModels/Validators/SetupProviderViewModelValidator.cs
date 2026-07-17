using FluentValidation;

namespace ZentraInstallerMVC.ViewModels.Validators;

public sealed class SetupProviderViewModelValidator : AbstractValidator<SetupProviderViewModel>
{
    public SetupProviderViewModelValidator()
    {
        RuleFor(model => model.Provider)
            .NotNull()
            .WithMessage("Database provider is required.");
    }
}
