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
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
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
        Test(
            (guid, provider) => MissingGuidMethods.Parse(guid.ToString(), provider),
            (guid, _) => System.Guid.Parse(guid.ToString()),
            TestValues,
            FormatProviders);
        Test(
            (guid, provider) => MissingGuidMethods.Parse(guid.ToString().AsSpan(), provider),
            (guid, _) => MissingGuidMethods.Parse(guid.ToString().AsSpan()),
            TestValues,
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestToString()
    {
        var formatsRedundant = new[] { null, "", "D", "d" };

        Test((guid, format) => guid.ToString(format), (n, _) => n.ToString(), TestValues, formatsRedundant);

        Test(
            (guid, format, provider) => guid.ToString(format, provider),
            (guid, format, _) => guid.ToString(format),
            TestValues,
            [..formatsRedundant, "N"],
            FormatProviders);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (System.Guid value, IFormatProvider? provider, out System.Guid result) => MissingGuidMethods.TryParse($"{value}", provider, out result),
            (System.Guid value, IFormatProvider? _, out System.Guid result) => System.Guid.TryParse($"{value}", out result),
            TestValues,
            FormatProviders);

        Test(
            (System.Guid value, IFormatProvider? provider, out System.Guid result)
                => MissingGuidMethods.TryParse($"{value}".AsSpan(), provider, out result),
            (System.Guid value, IFormatProvider? _, out System.Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), out result),
            TestValues,
            FormatProviders);

        DoNamedTest2();
    }
}