using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
public sealed class DateTimeAnalyzerTests : BaseTypeAnalyzerTests<System.DateTime>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseDateTimePropertySuggestion || highlighting.IsError();

    protected override System.DateTime[] TestValues { get; } =
    [
        System.DateTime.MinValue,
        System.DateTime.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
    ];

    [Test]
    public void TestDate() => DoNamedTest2();
}