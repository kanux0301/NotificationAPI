using Notification.Application.Common;
using Notification.Application.DTOs;

namespace Notification.Application.Templates.Queries.GetAllTemplates;

public record GetAllTemplatesQuery : IQuery<IReadOnlyList<NotificationTemplateDto>>;
