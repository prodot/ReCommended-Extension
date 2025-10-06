using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Half;

using half = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Half;

[TestFixture]
[TestNet50]
public sealed class HalfAnalyzerTests : BaseTypeAnalyzerTests<half>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Half";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion || highlighting.IsError();

    protected override half[] TestValues { get; } =
    [
        (sbyte)0,
        (sbyte)1,
        (sbyte)2,
        -1,
        -2,
        (half)(-0f),
        (half)1.2f,
        (half)(-1.2f),
        half.MaxValue,
        half.Epsilon,
        half.NaN,
        half.PositiveInfinity,
        half.NegativeInfinity,
    ];

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }
}