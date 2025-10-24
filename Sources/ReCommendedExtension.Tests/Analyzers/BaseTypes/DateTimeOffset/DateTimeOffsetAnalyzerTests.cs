using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTimeOffset;

[TestFixture]
public sealed class DateTimeOffsetAnalyzerTests : BaseTypeAnalyzerTests<System.DateTimeOffset>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTimeOffset";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion || highlighting.IsError();

    protected override System.DateTimeOffset[] TestValues { get; } =
    [
        System.DateTimeOffset.MinValue,
        System.DateTimeOffset.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.Zero),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(2)),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(-6)),
    ];

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test(dateTimeOffset => dateTimeOffset.Equals(null), _ => false);

        DoNamedTest2();
    }
}