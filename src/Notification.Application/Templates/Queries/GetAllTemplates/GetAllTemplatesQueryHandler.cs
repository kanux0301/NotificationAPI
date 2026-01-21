using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Application.Mappings;
using Notification.Domain.Repositories;

namespace Notification.Application.Templates.Queries.GetAllTemplates;

public class GetAllTemplatesQueryHandler : IQueryHandler<GetAllTemplatesQuery, IReadOnlyList<NotificationTemplateDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTemplatesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<NotificationTemplateDto>>> Handle(GetAllTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _unitOfWork.Templates.GetActiveTemplatesAsync(cancellationToken);
        return Result.Success(templates.ToDto());
    }
}
