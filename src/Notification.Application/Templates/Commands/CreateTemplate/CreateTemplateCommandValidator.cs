using FluentValidation;

namespace Notification.Application.Templates.Commands.CreateTemplate;

public class CreateTemplateCommandValidator : AbstractValidator<CreateTemplateCommand>
{
    public CreateTemplateCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Template name is required.")
            .MaximumLength(100).WithMessage("Template name cannot exceed 100 characters.");

        RuleFor(x => x.BodyTemplate)
            .NotEmpty().WithMessage("Body template is required.");

        RuleFor(x => x.SubjectTemplate)
            .MaximumLength(500).WithMessage("Subject template cannot exceed 500 characters.");
    }
}
