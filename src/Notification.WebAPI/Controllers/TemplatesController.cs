using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.DTOs;
using Notification.Application.Templates.Commands.CreateTemplate;
using Notification.Application.Templates.Queries.GetAllTemplates;
using Notification.Application.Templates.Queries.GetTemplate;
using Notification.Domain.Enums;

namespace Notification.WebAPI.Controllers;

public class TemplatesController : ApiController
{
    private readonly IMediator _mediator;

    public TemplatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new notification template
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTemplate(
        [FromBody] CreateTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateTemplateCommand
        {
            Name = request.Name,
            SubjectTemplate = request.SubjectTemplate,
            BodyTemplate = request.BodyTemplate,
            Channel = request.Channel,
            IsHtml = request.IsHtml
        };

        var result = await _mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetTemplate), new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    /// <summary>
    /// Get a template by ID
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetTemplate))]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplate(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTemplateQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get all active templates
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTemplates(CancellationToken cancellationToken)
    {
        var query = new GetAllTemplatesQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }
}

public record CreateTemplateRequest
{
    public string Name { get; init; } = null!;
    public string SubjectTemplate { get; init; } = null!;
    public string BodyTemplate { get; init; } = null!;
    public ChannelType Channel { get; init; }
    public bool IsHtml { get; init; }
}
