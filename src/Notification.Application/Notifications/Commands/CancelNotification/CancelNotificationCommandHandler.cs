using Notification.Application.Common;
using Notification.Domain.Repositories;

namespace Notification.Application.Notifications.Commands.CancelNotification;

public class CancelNotificationCommandHandler : ICommandHandler<CancelNotificationCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelNotificationCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);

        if (notification is null)
        {
            return Result.Failure(Error.NotFound("Notification", request.NotificationId));
        }

        try
        {
            notification.Cancel();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Validation("InvalidOperation", ex.Message));
        }
    }
}
