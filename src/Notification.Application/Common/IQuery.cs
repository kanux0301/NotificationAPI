using MediatR;

namespace Notification.Application.Common;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
