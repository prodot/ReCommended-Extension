using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Nullable;

[TestFixture]
public sealed class NullableAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Nullable";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseNullableHasValueAlternativeSuggestion or ReplaceNullableValueWithTypeCastSuggestion or UseBinaryOperatorSuggestion
            || highlighting.IsError();

    static void Test<T, R>(Func<T?, R> expected, Func<T?, R> actual, T value, bool useNull = true) where T : struct
    {
        if (useNull)
        {
            Assert.AreEqual(expected(null), actual(null));
        }

        Assert.AreEqual(expected(value), actual(value));
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    [SuppressMessage("ReSharper", "ArrangeNullCheckingPattern")]
    [SuppressMessage("ReSharper", "Solution.PatternMatchingNullCheck")]
    public void TestHasValue()
    {
        Test(nullable => nullable.HasValue, nullable => nullable is { }, 1);
        Test(nullable => nullable.HasValue, nullable => nullable is not null, 1);
        Test(nullable => nullable.HasValue, nullable => nullable != null, 1);
        Test(nullable => nullable.HasValue, nullable => nullable is (_, _), (1, true));

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestValue()
    {
        Test(nullable => nullable!.Value, nullable => (int)nullable!, 1, false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetValueOrDefault()
    {
        Test(nullable => nullable.GetValueOrDefault(), nullable => nullable ?? 0, 1);
        Test(nullable => nullable.GetValueOrDefault(1), nullable => nullable ?? 1, 1);

        DoNamedTest2();
    }
}