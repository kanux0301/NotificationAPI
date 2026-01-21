using Notification.Domain.Common;
using Notification.Domain.Enums;

namespace Notification.Domain.ValueObjects;

public class Recipient : ValueObject
{
    public string Address { get; }
    public ChannelType Channel { get; }
    public string? Name { get; }

    private Recipient(string address, ChannelType channel, string? name)
    {
        Address = address;
        Channel = channel;
        Name = name;
    }

    public static Recipient CreateEmail(EmailAddress email, string? name = null)
        => new(email.Value, ChannelType.Email, name);

    public static Recipient CreateSms(PhoneNumber phone, string? name = null)
        => new(phone.Value, ChannelType.Sms, name);

    public static Recipient CreatePush(string deviceToken, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(deviceToken))
            throw new ArgumentException("Device token cannot be empty.", nameof(deviceToken));

        return new Recipient(deviceToken, ChannelType.Push, name);
    }

    public static Recipient CreateWebhook(string webhookUrl, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(webhookUrl))
            throw new ArgumentException("Webhook URL cannot be empty.", nameof(webhookUrl));

        if (!Uri.TryCreate(webhookUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid webhook URL format.", nameof(webhookUrl));

        return new Recipient(webhookUrl, ChannelType.Webhook, name);
    }

    public static Recipient CreateInApp(string userId, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        return new Recipient(userId, ChannelType.InApp, name);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Address;
        yield return Channel;
        yield return Name;
    }

    public override string ToString() => Name is not null ? $"{Name} <{Address}>" : Address;
}
