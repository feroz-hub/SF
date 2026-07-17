using FluentValidation;

namespace ZentraInstallerMVC.ViewModels.Validators;

public sealed class SetupConnectionViewModelValidator : AbstractValidator<SetupConnectionViewModel>
{
    public SetupConnectionViewModelValidator()
    {
        RuleFor(model => model.Provider)
            .NotNull()
            .WithMessage("Database provider is required.");

        RuleFor(model => model.ConnectionString)
            .NotEmpty()
            .WithMessage("Connection string is required.")
            .MaximumLength(2048);
    }
}
