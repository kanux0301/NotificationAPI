using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Application.Mappings;
using Notification.Domain.Repositories;

namespace Notification.Application.Notifications.Queries.GetNotification;

public class GetNotificationQueryHandler : IQueryHandler<GetNotificationQuery, NotificationDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationDto>> Handle(GetNotificationQuery request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
        {
            return Result.Failure<NotificationDto>(Error.NotFound("Notification", request.NotificationId));
        }

        return Result.Success(notification.ToDto());
    }
}
