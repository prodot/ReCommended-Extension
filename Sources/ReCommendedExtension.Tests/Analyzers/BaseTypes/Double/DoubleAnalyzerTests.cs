using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Double;

[TestFixture]
public sealed class DoubleAnalyzerTests : BaseTypeAnalyzerTests<double>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Double";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseFloatingPointPatternSuggestion || highlighting.IsError();

    protected override double[] TestValues { get; } =
    [
        0,
        -0d,
        1,
        2,
        -1,
        -2,
        1.2,
        -1.2,
        double.MaxValue,
        double.MinValue,
        double.Epsilon,
        double.NaN,
        double.PositiveInfinity,
        double.NegativeInfinity,
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
        Test(number => number.GetTypeCode(), _ => TypeCode.Double);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseFloatingPointPattern")]
    public void TestIsNaN()
    {
        Test(number => double.IsNaN(number), number => number is double.NaN);

        DoNamedTest2();
    }
}