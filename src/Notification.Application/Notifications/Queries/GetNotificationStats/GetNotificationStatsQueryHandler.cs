using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Domain.Enums;
using Notification.Domain.Repositories;

namespace Notification.Application.Notifications.Queries.GetNotificationStats;

public class GetNotificationStatsQueryHandler : IQueryHandler<GetNotificationStatsQuery, NotificationStatsDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNotificationStatsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationStatsDto>> Handle(GetNotificationStatsQuery request, CancellationToken cancellationToken)
    {
        var pending = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Pending, cancellationToken);
        var processing = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Processing, cancellationToken);
        var sent = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Sent, cancellationToken);
        var delivered = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Delivered, cancellationToken);
        var failed = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Failed, cancellationToken);
        var cancelled = await _unitOfWork.Notifications.GetCountByStatusAsync(NotificationStatus.Cancelled, cancellationToken);

        var stats = new NotificationStatsDto
        {
            TotalPending = pending,
            TotalProcessing = processing,
            TotalSent = sent,
            TotalDelivered = delivered,
            TotalFailed = failed,
            TotalCancelled = cancelled
        };

        return Result.Success(stats);
    }
}
