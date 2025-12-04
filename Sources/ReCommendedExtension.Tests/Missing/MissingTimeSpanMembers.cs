using System.Globalization;
using System.Numerics;

namespace ReCommendedExtension.Tests.Missing;

internal static class MissingTimeSpanMembers
{
    extension(TimeSpan source)
    {
        [Pure]
        public static TimeSpan _Ctor(int days, int hours, int minutes, int seconds, int milliseconds, int microseconds)
        {
            const long ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1_000;

            return new TimeSpan(days, hours, minutes, seconds, milliseconds) + TimeSpan.FromTicks(microseconds * ticksPerMicrosecond);
        }

        [Pure]
        public TimeSpan Divide(double divisor) => source / divisor;

        public static TimeSpan operator /(TimeSpan timeSpan, double divisor)
        {
            if (divisor is double.NaN)
            {
                throw new ArgumentException($"Cannot divide by {nameof(double.NaN)}.", nameof(divisor));
            }

            return TimeSpan.FromTicks(checked((long)Math.Round(timeSpan.Ticks / divisor)));
        }

        [Pure]
        public double Divide(TimeSpan ts) => source / ts;

        public static double operator /(TimeSpan timeSpan, TimeSpan ts) => timeSpan.Ticks / (double)ts.Ticks;

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
        public static TimeSpan FromMilliseconds(long milliseconds)
        {
            var ticks = (BigInteger)milliseconds * TimeSpan.TicksPerMillisecond;

            if (ticks < long.MinValue || ticks > long.MaxValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            return TimeSpan.FromTicks((long)ticks);
        }

        [Pure]
        public static TimeSpan FromMilliseconds(long milliseconds, long microseconds)
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

        [Pure]
        public TimeSpan Multiply(double factor) => source * factor;

        public static TimeSpan operator *(TimeSpan timeSpan, double factor)
        {
            if (factor is double.NaN)
            {
                throw new ArgumentException($"Cannot multiply by {nameof(double.NaN)}.", nameof(factor));
            }

            return TimeSpan.FromTicks(checked((long)Math.Round(timeSpan.Ticks * factor)));
        }

        [Pure]
        public static TimeSpan ParseExact(
            ReadOnlySpan<char> input,
            string[] formats,
            IFormatProvider? formatProvider,
            TimeSpanStyles styles = TimeSpanStyles.None)
            => TimeSpan.ParseExact(input.ToString(), formats, formatProvider, styles);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> input, IFormatProvider? formatProvider, out TimeSpan result)
            => TimeSpan.TryParse(input.ToString(), formatProvider, out result);

        [Pure]
        public static bool TryParse(ReadOnlySpan<char> input, out TimeSpan result) => TimeSpan.TryParse(input.ToString(), out result);

        [Pure]
        public static bool TryParseExact(
            ReadOnlySpan<char> input,
            ReadOnlySpan<char> format,
            IFormatProvider? formatProvider,
            TimeSpanStyles styles,
            out TimeSpan result)
            => TimeSpan.TryParseExact(input.ToString(), format.ToString(), formatProvider, styles, out result);

        [Pure]
        public static bool TryParseExact(ReadOnlySpan<char> input, ReadOnlySpan<char> format, IFormatProvider? formatProvider, out TimeSpan result)
            => TimeSpan.TryParseExact(input.ToString(), format.ToString(), formatProvider, out result);

        [Pure]
        public static bool TryParseExact(ReadOnlySpan<char> input, string?[]? formats, IFormatProvider? formatProvider, out TimeSpan result)
            => TimeSpan.TryParseExact(input.ToString(), formats, formatProvider, out result);

        [Pure]
        public static bool TryParseExact(
            ReadOnlySpan<char> input,
            string?[]? formats,
            IFormatProvider? formatProvider,
            TimeSpanStyles styles,
            out TimeSpan result)
            => TimeSpan.TryParseExact(input.ToString(), formats, formatProvider, styles, out result);
    }
}