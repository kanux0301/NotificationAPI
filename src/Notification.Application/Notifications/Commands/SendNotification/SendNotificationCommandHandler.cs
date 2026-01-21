using Notification.Application.Common;
using Notification.Application.Common.Messaging;
using Notification.Application.Common.Messaging.Messages;
using Notification.Domain.Entities;
using Notification.Domain.Enums;
using Notification.Domain.Repositories;
using Notification.Domain.ValueObjects;

namespace Notification.Application.Notifications.Commands.SendNotification;

public class SendNotificationCommandHandler : ICommandHandler<SendNotificationCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessagePublisher _messagePublisher;

    public SendNotificationCommandHandler(
        IUnitOfWork unitOfWork,
        IMessagePublisher messagePublisher)
    {
        _unitOfWork = unitOfWork;
        _messagePublisher = messagePublisher;
    }

    public async Task<Result<Guid>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var recipient = CreateRecipient(request);
        var content = request.IsHtml
            ? NotificationContent.CreateHtml(request.Subject, request.Body)
            : NotificationContent.CreatePlainText(request.Subject, request.Body);

        var notification = NotificationMessage.Create(
            recipient,
            content,
            request.Priority,
            templateId: null,
            request.ScheduledAt);

        if (request.Metadata is not null)
        {
            foreach (var (key, value) in request.Metadata)
            {
                notification.AddMetadata(key, value);
            }
        }

        // Save notification to database with Pending status
        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish to message queue for channel microservice to process
        await PublishToQueueAsync(notification, request, cancellationToken);

        return Result.Success(notification.Id);
    }

    private async Task PublishToQueueAsync(
        NotificationMessage notification,
        SendNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var message = new SendNotificationMessage
        {
            NotificationId = notification.Id,
            Channel = request.Channel,
            Priority = request.Priority,
            Recipient = new RecipientInfo
            {
                Address = request.RecipientAddress,
                Name = request.RecipientName
            },
            Content = new ContentInfo
            {
                Subject = request.Subject,
                Body = request.Body,
                IsHtml = request.IsHtml
            },
            Metadata = request.Metadata ?? [],
            TemplateId = null
        };

        var queueName = GetQueueName(request.Channel);

        // If scheduled, publish with delay; otherwise publish immediately
        if (request.ScheduledAt.HasValue)
        {
            var delay = request.ScheduledAt.Value - DateTime.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                await _messagePublisher.PublishWithDelayAsync(message, queueName, delay, cancellationToken);
            }
            else
            {
                // Scheduled time is in the past, send immediately
                await _messagePublisher.PublishAsync(message, queueName, cancellationToken);
            }
        }
        else
        {
            await _messagePublisher.PublishAsync(message, queueName, cancellationToken);
        }
    }

    private static string GetQueueName(ChannelType channel) => channel switch
    {
        ChannelType.Email => QueueNames.EmailNotifications,
        ChannelType.Sms => QueueNames.SmsNotifications,
        ChannelType.Push => QueueNames.PushNotifications,
        ChannelType.Webhook => QueueNames.WebhookNotifications,
        ChannelType.InApp => QueueNames.InAppNotifications,
        _ => throw new ArgumentException($"Unsupported channel type: {channel}")
    };

    private static Recipient CreateRecipient(SendNotificationCommand request)
    {
        return request.Channel switch
        {
            ChannelType.Email => Recipient.CreateEmail(
                EmailAddress.Create(request.RecipientAddress),
                request.RecipientName),
            ChannelType.Sms => Recipient.CreateSms(
                PhoneNumber.Create(request.RecipientAddress),
                request.RecipientName),
            ChannelType.Push => Recipient.CreatePush(
                request.RecipientAddress,
                request.RecipientName),
            ChannelType.Webhook => Recipient.CreateWebhook(
                request.RecipientAddress,
                request.RecipientName),
            ChannelType.InApp => Recipient.CreateInApp(
                request.RecipientAddress,
                request.RecipientName),
            _ => throw new ArgumentException($"Unsupported channel type: {request.Channel}")
        };
    }
}
