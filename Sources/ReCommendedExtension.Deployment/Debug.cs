namespace ReCommendedExtension.Deployment;

internal static class Debug
{
    [Conditional("DEBUG")]
    [AssertionMethod]
    public static void Assert([AssertionCondition(AssertionConditionType.IS_TRUE)][DoesNotReturnIf(false)] bool condition)
        => System.Diagnostics.Debug.Assert(condition);
}