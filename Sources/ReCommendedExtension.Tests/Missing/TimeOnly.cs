using NUnit.Framework;

namespace ReCommendedExtension.Tests.Missing;

public readonly record struct TimeOnly
{
    [Pure]
    public static TimeOnly FromDateTime(DateTime dateTime) => new(dateTime.TimeOfDay);

    readonly TimeSpan timeOfDay;

    TimeOnly(TimeSpan timeOfDay)
    {
        Assert.GreaterOrEqual(timeOfDay, TimeSpan.Zero);
        Assert.Less(timeOfDay, TimeSpan.FromHours(24));

        this.timeOfDay = timeOfDay;
    }

    [Pure]
    public TimeSpan ToTimeSpan() => timeOfDay;
}