using Notification.Domain.Enums;

namespace Notification.Domain.Services;

public interface INotificationSenderFactory
{
    INotificationSender GetSender(ChannelType channel);
    bool HasSender(ChannelType channel);
}
