using FluentValidation;
using Notification.Domain.Enums;

namespace Notification.Application.Notifications.Commands.SendNotification;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {
        RuleFor(x => x.RecipientAddress)
            .NotEmpty().WithMessage("Recipient address is required.");

        RuleFor(x => x.RecipientAddress)
            .EmailAddress().WithMessage("Invalid email address format.")
            .When(x => x.Channel == ChannelType.Email);

        RuleFor(x => x.RecipientAddress)
            .Matches(@"^\+?[1-9]\d{6,14}$").WithMessage("Invalid phone number format.")
            .When(x => x.Channel == ChannelType.Sms);

        RuleFor(x => x.RecipientAddress)
            .Must(BeValidUrl).WithMessage("Invalid webhook URL format.")
            .When(x => x.Channel == ChannelType.Webhook);

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Notification body is required.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required for email notifications.")
            .When(x => x.Channel == ChannelType.Email);

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow).WithMessage("Scheduled time must be in the future.")
            .When(x => x.ScheduledAt.HasValue);
    }

    private static bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
