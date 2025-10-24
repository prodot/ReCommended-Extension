using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateOnly;

[TestFixture]
[TestNet60]
public sealed class DateOnlyAnalyzerTests : BaseTypeAnalyzerTests<Missing.DateOnly>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateOnly";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion || highlighting.IsError();

    protected override Missing.DateOnly[] TestValues { get; } = [Missing.DateOnly.MinValue, Missing.DateOnly.MaxValue, new(2025, 7, 15)];

    [Test]
    public void TestEquals()
    {
        Test(dateOnly => dateOnly.Equals(null), _ => false);

        DoNamedTest2();
    }
}