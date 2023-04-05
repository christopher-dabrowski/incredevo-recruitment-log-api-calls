namespace functionApp.services;

public interface IClock
{
    DateTime DateTimeNow { get; }
    DateTimeOffset DateTimeOffsetNow { get; }
}
