using Notification.Application.DTOs;
using Notification.Domain.Entities;

namespace Notification.Application.Mappings;

public static class NotificationMappings
{
    public static NotificationDto ToDto(this NotificationMessage entity)
    {
        return new NotificationDto
        {
            Id = entity.Id,
            RecipientAddress = entity.Recipient.Address,
            RecipientName = entity.Recipient.Name,
            Channel = entity.Recipient.Channel,
            Subject = entity.Content.Subject,
            Body = entity.Content.Body,
            Status = entity.Status,
            Priority = entity.Priority,
            TemplateId = entity.TemplateId,
            ScheduledAt = entity.ScheduledAt,
            SentAt = entity.SentAt,
            DeliveredAt = entity.DeliveredAt,
            FailureReason = entity.FailureReason,
            RetryCount = entity.RetryCount,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Metadata = entity.Metadata
        };
    }

    public static IReadOnlyList<NotificationDto> ToDto(this IEnumerable<NotificationMessage> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }

    public static NotificationTemplateDto ToDto(this NotificationTemplate entity)
    {
        return new NotificationTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            SubjectTemplate = entity.SubjectTemplate,
            BodyTemplate = entity.BodyTemplate,
            Channel = entity.Channel,
            IsHtml = entity.IsHtml,
            IsActive = entity.IsActive,
            RequiredVariables = entity.RequiredVariables,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static IReadOnlyList<NotificationTemplateDto> ToDto(this IEnumerable<NotificationTemplate> entities)
    {
        return entities.Select(e => e.ToDto()).ToList();
    }
}
