using MediatR;
using Microsoft.AspNetCore.Mvc;
using Notification.Application.DTOs;
using Notification.Application.Notifications.Commands.CancelNotification;
using Notification.Application.Notifications.Commands.RetryNotification;
using Notification.Application.Notifications.Commands.SendNotification;
using Notification.Application.Notifications.Queries.GetNotification;
using Notification.Application.Notifications.Queries.GetNotificationsByStatus;
using Notification.Application.Notifications.Queries.GetNotificationStats;
using Notification.Domain.Enums;

namespace Notification.WebAPI.Controllers;

public class NotificationsController : ApiController
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Send a new notification
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SendNotification(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SendNotificationCommand
        {
            RecipientAddress = request.RecipientAddress,
            RecipientName = request.RecipientName,
            Channel = request.Channel,
            Subject = request.Subject,
            Body = request.Body,
            IsHtml = request.IsHtml,
            Priority = request.Priority,
            ScheduledAt = request.ScheduledAt,
            Metadata = request.Metadata
        };

        var result = await _mediator.Send(command, cancellationToken);

        return HandleCreatedResult(result, nameof(GetNotification), new { id = result.IsSuccess ? result.Value : Guid.Empty });
    }

    /// <summary>
    /// Get a notification by ID
    /// </summary>
    [HttpGet("{id:guid}", Name = nameof(GetNotification))]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNotification(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetNotificationQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get notifications by status
    /// </summary>
    [HttpGet("status/{status}")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStatus(NotificationStatus status, CancellationToken cancellationToken)
    {
        var query = new GetNotificationsByStatusQuery(status);
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Get notification statistics
    /// </summary>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(NotificationStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var query = new GetNotificationStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Cancel a pending notification
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelNotification(Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelNotificationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Retry a failed notification
    /// </summary>
    [HttpPost("{id:guid}/retry")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RetryNotification(Guid id, CancellationToken cancellationToken)
    {
        var command = new RetryNotificationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return HandleResult(result);
    }
}

public record SendNotificationRequest
{
    public string RecipientAddress { get; init; } = null!;
    public string? RecipientName { get; init; }
    public ChannelType Channel { get; init; }
    public string Subject { get; init; } = null!;
    public string Body { get; init; } = null!;
    public bool IsHtml { get; init; }
    public NotificationPriority Priority { get; init; } = NotificationPriority.Normal;
    public DateTime? ScheduledAt { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}
