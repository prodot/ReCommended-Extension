using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.DateTime;

[TestFixture]
public sealed class DateTimeAnalyzerTests : BaseTypeAnalyzerTests<System.DateTime>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\DateTime";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseDateTimePropertySuggestion || highlighting.IsError();

    protected override System.DateTime[] TestValues { get; } =
    [
        System.DateTime.MinValue,
        System.DateTime.MaxValue,
        new(2025, 7, 15, 21, 33, 0, 123),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Local),
        new(2025, 7, 15, 21, 33, 0, 123, DateTimeKind.Utc),
    ];

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet80]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    [SuppressMessage("ReSharper", "RedundantArgumentRange")]
    public void Test_Constructors()
    {
        Test(() => new System.DateTime(0), () => System.DateTime.MinValue);

        DoNamedTest2();
    }

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp60)]
    public void TestDate() => DoNamedTest2();

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test(dateTime => dateTime.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(dateTime => dateTime.GetTypeCode(), _ => TypeCode.DateTime);

        DoNamedTest2();
    }
}