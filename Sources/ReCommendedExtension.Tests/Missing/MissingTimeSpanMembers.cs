using System.Numerics;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingTimeSpanMembers
{
    [Pure]
    public static TimeSpan _Ctor(int days, int hours, int minutes, int seconds, int milliseconds, int microseconds)
    {
        const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

        return new TimeSpan(days, hours, minutes, seconds, milliseconds) + TimeSpan.FromTicks(microseconds * ticksPerMicrosecond);
    }

    [Pure]
    public static TimeSpan Divide(this TimeSpan timeSpan, double divisor) => op_Divide(timeSpan, divisor);

    [Pure]
    public static TimeSpan op_Divide(TimeSpan timeSpan, double divisor) // todo: use extension operator when available
    {
        if (divisor is double.NaN)
        {
            throw new ArgumentException($"Cannot divide by {nameof(double.NaN)}.", nameof(divisor));
        }

        return TimeSpan.FromTicks(checked((long)Math.Round(timeSpan.Ticks / divisor)));
    }

    [Pure]
    public static double Divide(this TimeSpan timeSpan, TimeSpan ts) => op_Divide(timeSpan, ts);

    [Pure]
    public static double op_Divide(TimeSpan timeSpan, TimeSpan ts) => timeSpan.Ticks / (double)ts.Ticks; // todo: use extension operator when available

    [Pure]
    public static TimeSpan FromDays(int days)
    {
        if ((long)days is < long.MinValue / TimeSpan.TicksPerDay or > long.MaxValue / TimeSpan.TicksPerDay)
        {
            throw new ArgumentOutOfRangeException(nameof(days));
        }

        return TimeSpan.FromTicks(days * TimeSpan.TicksPerDay);
    }

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeSpan FromDays(int days, int hours = 0, long minutes = 0, long seconds = 0, long milliseconds = 0, long microseconds = 0)
    {
        var ticks = (BigInteger)days * TimeSpan.TicksPerDay
            + (BigInteger)hours * TimeSpan.TicksPerHour
            + (BigInteger)minutes * TimeSpan.TicksPerMinute
            + (BigInteger)seconds * TimeSpan.TicksPerSecond
            + (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond
            + (BigInteger)microseconds * (TimeSpan.TicksPerMillisecond / 1_000);

        if (ticks < long.MinValue || ticks > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        return TimeSpan.FromTicks((long)ticks);
    }

    [Pure]
    public static TimeSpan FromHours(int hours)
    {
        if ((long)hours is < long.MinValue / TimeSpan.TicksPerHour or > long.MaxValue / TimeSpan.TicksPerHour)
        {
            throw new ArgumentOutOfRangeException(nameof(hours));
        }

        return TimeSpan.FromTicks(hours * TimeSpan.TicksPerHour);
    }

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeSpan FromHours(int hours, long minutes = 0, long seconds = 0, long milliseconds = 0, long microseconds = 0)
    {
        var ticks = (BigInteger)hours * TimeSpan.TicksPerHour
            + (BigInteger)minutes * TimeSpan.TicksPerMinute
            + (BigInteger)seconds * TimeSpan.TicksPerSecond
            + (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond
            + (BigInteger)microseconds * (TimeSpan.TicksPerMillisecond / 1_000);

        if (ticks < long.MinValue || ticks > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        return TimeSpan.FromTicks((long)ticks);
    }

    [Pure]
    public static TimeSpan FromMicroseconds(long microseconds)
    {
        const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

        if (microseconds is < long.MinValue / ticksPerMicrosecond or > long.MaxValue / ticksPerMicrosecond)
        {
            throw new ArgumentOutOfRangeException(nameof(microseconds));
        }

        return TimeSpan.FromTicks(microseconds * ticksPerMicrosecond);
    }

    [Pure]
    public static TimeSpan FromMilliseconds(long milliseconds, long microseconds = 0)
    {
        var ticks = (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond + (BigInteger)microseconds * (TimeSpan.TicksPerMillisecond / 1_000);

        if (ticks < long.MinValue || ticks > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        return TimeSpan.FromTicks((long)ticks);
    }

    [Pure]
    public static TimeSpan FromMinutes(long minutes)
    {
        if (minutes is < long.MinValue / TimeSpan.TicksPerMinute or > long.MaxValue / TimeSpan.TicksPerMinute)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes));
        }

        return TimeSpan.FromTicks(minutes * TimeSpan.TicksPerMinute);
    }

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeSpan FromMinutes(long minutes, long seconds = 0, long milliseconds = 0, long microseconds = 0)
    {
        var ticks = (BigInteger)minutes * TimeSpan.TicksPerMinute
            + (BigInteger)seconds * TimeSpan.TicksPerSecond
            + (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond
            + (BigInteger)microseconds * (TimeSpan.TicksPerMillisecond / 1_000);

        if (ticks < long.MinValue || ticks > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        return TimeSpan.FromTicks((long)ticks);
    }

    [Pure]
    public static TimeSpan FromSeconds(long seconds)
    {
        if (seconds is < long.MinValue / TimeSpan.TicksPerSecond or > long.MaxValue / TimeSpan.TicksPerSecond)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds));
        }

        return TimeSpan.FromTicks(seconds * TimeSpan.TicksPerSecond);
    }

    [Pure]
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public static TimeSpan FromSeconds(long seconds, long milliseconds = 0, long microseconds = 0)
    {
        var ticks = (BigInteger)seconds * TimeSpan.TicksPerSecond
            + (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond
            + (BigInteger)microseconds * (TimeSpan.TicksPerMillisecond / 1_000);

        if (ticks < long.MinValue || ticks > long.MaxValue)
        {
            throw new ArgumentOutOfRangeException();
        }

        return TimeSpan.FromTicks((long)ticks);
    }
}