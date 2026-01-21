using Notification.Application.Common;
using Notification.Domain.Repositories;

namespace Notification.Application.Notifications.Commands.RetryNotification;

public class RetryNotificationCommandHandler : ICommandHandler<RetryNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RetryNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RetryNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
        {
            return Result.Failure(Error.NotFound("Notification", request.NotificationId));
        }

        if (!notification.CanRetry())
        {
            return Result.Failure(Error.Validation("CannotRetry",
                $"Notification cannot be retried. Status: {notification.Status}, Retry count: {notification.RetryCount}"));
        }

        try
        {
            notification.Retry();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation("InvalidOperation", ex.Message));
        }
    }
}
