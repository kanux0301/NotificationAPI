using Notification.Application.Common;
using Notification.Domain.Entities;
using Notification.Domain.Repositories;

namespace Notification.Application.Templates.Commands.CreateTemplate;

public class CreateTemplateCommandHandler : ICommandHandler<CreateTemplateCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTemplateCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        var existingTemplate = await _unitOfWork.Templates.ExistsByNameAsync(request.Name, cancellationToken);

        if (existingTemplate)
        {
            return Result.Failure<Guid>(Error.Conflict("TemplateNameExists",
                $"A template with name '{request.Name}' already exists."));
        }

        var template = NotificationTemplate.Create(
            request.Name,
            request.SubjectTemplate,
            request.BodyTemplate,
            request.Channel,
            request.IsHtml);

        await _unitOfWork.Templates.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(template.Id);
    }
}
