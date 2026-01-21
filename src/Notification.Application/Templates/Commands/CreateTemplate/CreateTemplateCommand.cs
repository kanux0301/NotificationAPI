using Notification.Application.Common;
using Notification.Domain.Enums;

namespace Notification.Application.Templates.Commands.CreateTemplate;

public record CreateTemplateCommand : ICommand<Guid>
{
    public string Name { get; init; } = null!;
    public string SubjectTemplate { get; init; } = null!;
    public string BodyTemplate { get; init; } = null!;
    public ChannelType Channel { get; init; }
    public bool IsHtml { get; init; }
}
