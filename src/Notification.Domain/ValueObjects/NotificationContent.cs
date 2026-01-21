using Notification.Domain.Common;

namespace Notification.Domain.ValueObjects;

public class NotificationContent : ValueObject
{
    public string Subject { get; }
    public string Body { get; }
    public bool IsHtml { get; }

    private NotificationContent(string subject, string body, bool isHtml)
    {
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }

    public static NotificationContent Create(string subject, string body, bool isHtml = false)
    {
        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Notification body cannot be empty.", nameof(body));

        return new NotificationContent(subject?.Trim() ?? string.Empty, body.Trim(), isHtml);
    }

    public static NotificationContent CreatePlainText(string subject, string body)
        => Create(subject, body, false);

    public static NotificationContent CreateHtml(string subject, string body)
        => Create(subject, body, true);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Subject;
        yield return Body;
        yield return IsHtml;
    }
}
