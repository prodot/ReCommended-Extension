using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.TimeOnly;

[TestFixture]
[TestNet60]
public sealed class TimeOnlyAnalyzerTests : BaseTypeAnalyzerTests<Missing.TimeOnly>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\TimeOnly";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion || highlighting.IsError();

    protected override Missing.TimeOnly[] TestValues { get; } =
    [
        Missing.TimeOnly.MinValue, Missing.TimeOnly.MaxValue, new(0, 0, 1), new(0, 1, 0), new(1, 0, 0), new(1, 2, 3, 4, 5),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet70]
    public void Test_Constructors()
    {
        Test(() => new Missing.TimeOnly(0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0, 0), () => Missing.TimeOnly.MinValue);
        Test(() => new Missing.TimeOnly(0, 0, 0, 0, 0), () => Missing.TimeOnly.MinValue);

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((timeOnly, value) => timeOnly.Equals(value), (timeSpan, value) => timeSpan == value, TestValues, TestValues);
        Test(timeOnly => timeOnly.Equals(null), _ => false);

        DoNamedTest2();
    }
}