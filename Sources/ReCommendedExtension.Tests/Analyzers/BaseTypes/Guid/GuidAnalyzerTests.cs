using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Guid;

[TestFixture]
public sealed class GuidAnalyzerTests : BaseTypeAnalyzerTests<System.Guid>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Guid";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseBinaryOperatorSuggestion or UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override System.Guid[] TestValues { get; } = [System.Guid.Empty, new([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16])];

    [Test]
    public void TestEquals()
    {
        Test((guid, g) => guid.Equals(g), (guid, g) => guid == g, TestValues, TestValues);

        Test(guid => guid.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestParse()
    {
        Test(guid => MissingGuidMethods.Parse(guid.ToString(), CultureInfo.CurrentCulture), guid => System.Guid.Parse(guid.ToString()));
        Test(
            guid => MissingGuidMethods.Parse(guid.ToString().AsSpan(), CultureInfo.CurrentCulture),
            guid => MissingGuidMethods.Parse(guid.ToString().AsSpan()));

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test(guid => guid.ToString(null), guid => guid.ToString());
        Test(guid => guid.ToString(""), guid => guid.ToString());
        Test(guid => guid.ToString("D"), guid => guid.ToString());
        Test(guid => guid.ToString("d"), guid => guid.ToString());

        Test(guid => guid.ToString("N", CultureInfo.CurrentCulture), guid => guid.ToString("N"));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}", CultureInfo.CurrentCulture, out result),
            (System.Guid value, out System.Guid result) => System.Guid.TryParse($"{value}", out result));

        Test(
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), CultureInfo.CurrentCulture, out result),
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), out result));

        DoNamedTest2();
    }
}