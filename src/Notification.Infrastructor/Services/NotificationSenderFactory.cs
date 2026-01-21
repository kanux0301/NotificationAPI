using Notification.Domain.Enums;
using Notification.Domain.Services;

namespace Notification.Infrastructure.Services;

public class NotificationSenderFactory : INotificationSenderFactory
{
    private readonly IEnumerable<INotificationSender> _senders;

    public NotificationSenderFactory(IEnumerable<INotificationSender> senders)
    {
        _senders = senders;
    }

    public INotificationSender GetSender(ChannelType channel)
    {
        var sender = _senders.FirstOrDefault(s => s.Channel == channel);

        if (sender is null)
        {
            throw new InvalidOperationException($"No notification sender registered for channel: {channel}");
        }

        return sender;
    }

    public bool HasSender(ChannelType channel)
    {
        return _senders.Any(s => s.Channel == channel);
    }
}
