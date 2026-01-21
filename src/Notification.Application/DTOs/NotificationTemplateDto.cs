using Notification.Domain.Enums;

namespace Notification.Application.DTOs;

public record NotificationTemplateDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string SubjectTemplate { get; init; } = null!;
    public string BodyTemplate { get; init; } = null!;
    public ChannelType Channel { get; init; }
    public bool IsHtml { get; init; }
    public bool IsActive { get; init; }
    public List<string> RequiredVariables { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}
