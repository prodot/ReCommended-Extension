using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Single;

[TestFixture]
public sealed class SingleAnalyzerTests : BaseTypeAnalyzerTests<float>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Single";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseFloatingPointPatternSuggestion || highlighting.IsError();

    protected override float[] TestValues { get; } =
    [
        0,
        -0f,
        1,
        2,
        -1,
        -2,
        1.2f,
        -1.2f,
        float.MinValue,
        float.MaxValue,
        float.Epsilon,
        float.NaN,
        float.PositiveInfinity,
        float.NegativeInfinity,
    ];

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Single);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseFloatingPointPattern")]
    public void TestIsNaN()
    {
        Test(number => float.IsNaN(number), number => number is float.NaN);

        DoNamedTest2();
    }
}