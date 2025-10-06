using System.Runtime.CompilerServices;
using JetBrains.ReSharper.Psi;

namespace ReCommendedExtension.Extensions.MethodFinding;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Using original type names.")]
[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "Used for debugging only.")]
internal sealed record Parameter
#if DEBUG
    ([CallerMemberName] string Name = "")
#endif
{
    public static Parameter T { get; } = new() { IsType = _ => true };

    public static Parameter Type { get; } = new() { IsType = t => t.IsSystemType() };

    public static Parameter Object { get; } = new() { IsType = t => t.IsObject() };

    public static Parameter ObjectArray { get; } = new() { IsType = t => t.IsGenericArrayOfObject() };

    public static Parameter String { get; } = new() { IsType = t => t.IsString() };

    public static Parameter StringArray { get; } = new() { IsType = t => t.IsGenericArrayOfString() };

    public static Parameter Char { get; } = new() { IsType = t => t.IsChar() };

    public static Parameter CharArray { get; } = new() { IsType = t => t.IsGenericArrayOfChar() };

    public static Parameter Boolean { get; } = new() { IsType = t => t.IsBool() };

    public static Parameter Byte { get; } = new() { IsType = t => t.IsByte() };

    public static Parameter SByte { get; } = new() { IsType = t => t.IsSbyte() };

    public static Parameter Int16 { get; } = new() { IsType = t => t.IsShort() };

    public static Parameter UInt16 { get; } = new() { IsType = t => t.IsUshort() };

    public static Parameter Int32 { get; } = new() { IsType = t => t.IsInt() };

    public static Parameter UInt32 { get; } = new() { IsType = t => t.IsUint() };

    public static Parameter Int64 { get; } = new() { IsType = t => t.IsLong() };

    public static Parameter UInt64 { get; } = new() { IsType = t => t.IsUlong() };

    public static Parameter Int128 { get; } = new() { IsType = t => t.IsInt128() };

    public static Parameter UInt128 { get; } = new() { IsType = t => t.IsUInt128() };

    public static Parameter IntPtr { get; } = new() { IsType = t => t.IsIntPtr() };

    public static Parameter UIntPtr { get; } = new() { IsType = t => t.IsUIntPtr() };

    public static Parameter Decimal { get; } = new() { IsType = t => t.IsDecimal() };

    public static Parameter Double { get; } = new() { IsType = t => t.IsDouble() };

    public static Parameter Single { get; } = new() { IsType = t => t.IsFloat() };

    public static Parameter Half { get; } = new() { IsType = t => t.IsHalf() };

    public static Parameter Guid { get; } = new() { IsType = t => t.IsGuid() };

    public static Parameter TimeSpan { get; } = new() { IsType = t => t.IsTimeSpan() };

    public static Parameter DateTime { get; } = new() { IsType = t => t.IsDateTime() };

    public static Parameter DateTimeOffset { get; } = new() { IsType = t => t.IsDateTimeOffset() };

    public static Parameter DateOnly { get; } = new() { IsType = t => t.IsDateOnly() };

    public static Parameter TimeOnly { get; } = new() { IsType = t => t.IsTimeOnly() };

    public static Parameter ReadOnlySpanOfString { get; } = new() { IsType = t => t.IsReadOnlySpanOfString() };

    public static Parameter ReadOnlySpanOfObject { get; } = new() { IsType = t => t.IsReadOnlySpanOfString() };

    public static Parameter ReadOnlySpanOfChar { get; } = new() { IsType = t => t.IsReadOnlySpanOfChar() };

    public static Parameter ReadOnlySpanOfByte { get; } = new() { IsType = t => t.IsReadOnlySpanOfByte() };

    public static Parameter IEnumerableOfT { get; } = new() { IsType = t => t.IsGenericIEnumerable() };

    public static Parameter NumberStyles { get; } = new() { IsType = t => t.IsNumberStyles() };

    public static Parameter IFormatProvider { get; } = new() { IsType = t => t.IsIFormatProvider() };

    public static Parameter StringComparison { get; } = new() { IsType = t => t.IsStringComparison() };

    public static Parameter StringSplitOptions { get; } = new() { IsType = t => t.IsStringSplitOptions() };

    public static Parameter MidpointRounding { get; } = new() { IsType = t => t.IsMidpointRounding() };

    public static Parameter DateTimeStyles { get; } = new() { IsType = t => t.IsDateTimeStyles() };

    public static Parameter DateTimeKind { get; } = new() { IsType = t => t.IsDateTimeKind() };

    public static Parameter TimeSpanStyles { get; } = new() { IsType = t => t.IsTimeSpanStyles() };

    public static Parameter Calendar { get; } = new() { IsType = t => t.IsCalendar() };

    public required Func<IType, bool> IsType { get; init; }

    public ParameterKind Kind { get; init; } = ParameterKind.VALUE;
}