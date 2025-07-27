using NUnit.Framework;

namespace ReCommendedExtension.Tests.Missing;

internal readonly record struct DateOnly
{
    [Pure]
    public static DateOnly FromDateTime(DateTime dateTime) => new(dateTime.Date);

    readonly DateTime date;

    DateOnly(DateTime date)
    {
        Assert.AreEqual(TimeSpan.Zero, date.TimeOfDay);

        this.date = date;
    }

    [Pure]
    public DateTime ToDateTime(TimeOnly time) => date + time.ToTimeSpan();
}