namespace Amethyst.Systems.Users.Telemetry;

public record UserSessionInfo(DateTime startTime, DateTime endTime)
{
    public DateTime Start { get; init; } = startTime;
    public DateTime End { get; init; } = endTime;
    public TimeSpan Duration => End - Start;
}