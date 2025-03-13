using System.Runtime.CompilerServices;

namespace ReCommendedExtension;

internal static class Debug
{
    [Conditional("DEBUG")]
    [AssertionMethod]
    public static void Assert(
        [AssertionCondition(AssertionConditionType.IS_TRUE)][DoesNotReturnIf(false)] bool condition,
        [CallerArgumentExpression(nameof(condition))] string message = "")
        => System.Diagnostics.Debug.Assert(condition, message);
}