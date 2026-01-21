using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Application.Mappings;
using Notification.Domain.Repositories;

namespace Notification.Application.Notifications.Queries.GetNotificationsByStatus;

public class GetNotificationsByStatusQueryHandler : IQueryHandler<GetNotificationsByStatusQuery, IReadOnlyList<NotificationDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationsByStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsByStatusQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _unitOfWork.Notifications.GetByStatusAsync(request.Status, cancellationToken);
        return Result.Success(notifications.ToDto());
    }
}
