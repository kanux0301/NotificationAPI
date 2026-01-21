using Notification.Application.Common;
using Notification.Application.DTOs;
using Notification.Application.Mappings;
using Notification.Domain.Repositories;

namespace Notification.Application.Templates.Queries.GetTemplate;

public class GetTemplateQueryHandler : IQueryHandler<GetTemplateQuery, NotificationTemplateDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTemplateQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<NotificationTemplateDto>> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
    {
        var template = await _unitOfWork.Templates.GetByIdAsync(request.TemplateId, cancellationToken);

        if (template is null)
        {
            return Result.Failure<NotificationTemplateDto>(Error.NotFound("Template", request.TemplateId));
        }

        return Result.Success(template.ToDto());
    }
}
