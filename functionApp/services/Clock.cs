namespace functionApp.services;

public class Clock : IClock
{
    public DateTime DateTimeNow => DateTime.Now;
    public DateTimeOffset DateTimeOffsetNow => DateTimeOffset.Now;
}
