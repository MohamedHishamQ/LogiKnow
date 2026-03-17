using FluentValidation;
using LogiKnow.Application.Common.DTOs;

namespace LogiKnow.Application.Common.Validators;

public class CreateTermValidator : AbstractValidator<CreateTermRequest>
{
    public CreateTermValidator()
    {
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DefinitionEn).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DefinitionAr).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.NameFr).MaximumLength(200);
        RuleFor(x => x.ExampleEn).MaximumLength(500);
        RuleFor(x => x.ExampleAr).MaximumLength(500);
    }
}

public class UpdateTermValidator : AbstractValidator<UpdateTermRequest>
{
    public UpdateTermValidator()
    {
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(200);
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DefinitionEn).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DefinitionAr).NotEmpty().MaximumLength(2000);
    }
}

public class AddBookValidator : AbstractValidator<AddBookRequest>
{
    public AddBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Authors).NotEmpty();
        RuleFor(x => x.Language).NotEmpty().MaximumLength(10);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ISBN).MaximumLength(20);
    }
}

public class SubmitAcademicEntryValidator : AbstractValidator<SubmitAcademicEntryRequest>
{
    public SubmitAcademicEntryValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
        RuleFor(x => x.University).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Year).InclusiveBetween(1900, 2100);
        RuleFor(x => x.Abstract).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Type).NotEmpty()
            .Must(t => new[] { "Thesis", "Dissertation", "Project" }.Contains(t))
            .WithMessage("Type must be Thesis, Dissertation, or Project.");
    }
}

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        RuleFor(x => x.PreferredLanguage)
            .Must(l => new[] { "ar", "en", "fr" }.Contains(l))
            .WithMessage("PreferredLanguage must be ar, en, or fr.");
    }
}

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class ReviewSubmissionValidator : AbstractValidator<ReviewSubmissionRequest>
{
    public ReviewSubmissionValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => !x.Approve)
            .WithMessage("Reason is required when rejecting a submission.");
    }
}
