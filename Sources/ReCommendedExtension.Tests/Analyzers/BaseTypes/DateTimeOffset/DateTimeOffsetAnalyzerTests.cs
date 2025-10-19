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
        => highlighting is UseBinaryOperatorSuggestion or UseExpressionResultSuggestion || highlighting.IsError();

    protected override System.DateTimeOffset[] TestValues { get; } =
    [
        System.DateTimeOffset.MinValue,
        System.DateTimeOffset.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.Zero),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(2)),
        new(2025, 7, 15, 21, 33, 0, 123, System.TimeSpan.FromHours(-6)),
    ];

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestAdd()
    {
        var value = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(dateTimeOffset => dateTimeOffset.Add(value), dateTime => dateTime + value, [..TestValues.Except([System.DateTimeOffset.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((dateTimeOffset, other) => dateTimeOffset.Equals(other), (dateTimeOffset, other) => dateTimeOffset == other, TestValues, TestValues);
        Test(dateTimeOffset => dateTimeOffset.Equals(null), _ => false);
        Test((first, second) => System.DateTimeOffset.Equals(first, second), (first, second) => first == second, TestValues, TestValues);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestSubtract()
    {
        var dateTimeOffsetValue = new System.DateTimeOffset(2021, 7, 21, 13, 08, 52, System.TimeSpan.FromHours(2));
        var timeSpanValue = new System.TimeSpan(1, 2, 3, 4, 5);

        Test(
            dateTimeOffset => dateTimeOffset.Subtract(dateTimeOffsetValue),
            dateTimeOffset => dateTimeOffset - dateTimeOffsetValue,
            [..TestValues.Except([System.DateTimeOffset.MinValue])]);
        Test(
            dateTimeOffset => dateTimeOffset.Subtract(timeSpanValue),
            dateTimeOffset => dateTimeOffset - timeSpanValue,
            [..TestValues.Except([System.DateTimeOffset.MinValue])]);

        DoNamedTest2();
    }
}