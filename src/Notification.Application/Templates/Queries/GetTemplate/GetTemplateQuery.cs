using Notification.Application.Common;
using Notification.Application.DTOs;

namespace Notification.Application.Templates.Queries.GetTemplate;

public record GetTemplateQuery(Guid TemplateId) : IQuery<NotificationTemplateDto>;
