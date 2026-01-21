namespace Notification.Application.DTOs;

public record NotificationStatsDto
{
    public int TotalPending { get; init; }
    public int TotalProcessing { get; init; }
    public int TotalSent { get; init; }
    public int TotalDelivered { get; init; }
    public int TotalFailed { get; init; }
    public int TotalCancelled { get; init; }
    public int Total => TotalPending + TotalProcessing + TotalSent + TotalDelivered + TotalFailed + TotalCancelled;
}
