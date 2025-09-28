using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UIntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UIntPtrAnalyzerTests : BaseTypeAnalyzerTests<nuint>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UIntPtr";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override nuint[] TestValues { get; } = [0, 1, 2];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUIntPtrMethods.Clamp(number, 1, 1), _ => 1u);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1u);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUIntPtrMethods.DivRem(0, 10), () => (0u, 0u));

        Test(() => MissingMathMethods.DivRem((nuint)0, 10), () => (0u, 0u));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUIntPtrMethods.Max(n, n), n => n);
        Test(n => MissingMathMethods.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUIntPtrMethods.Min(n, n), n => n);
        Test(n => MissingMathMethods.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer), n => MissingUIntPtrMethods.Parse($"{n}"));
        Test(n => MissingUIntPtrMethods.Parse($"{n}", null), n => MissingUIntPtrMethods.Parse($"{n}"));
        Test(
            (n, provider) => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => MissingUIntPtrMethods.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.None, null), n => MissingUIntPtrMethods.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUIntPtrMethods.Parse($"{n}".AsSpan(), null), n => MissingUIntPtrMethods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingUIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingUIntPtrMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUIntPtrMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUIntPtrMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (nuint n, IFormatProvider? provider, out nuint result)
                => MissingUIntPtrMethods.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}", out result));

        Test(
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (nuint n, IFormatProvider? provider, out nuint result) => MissingUIntPtrMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                provider,
                out result),
            TestValues,
            FormatProviders);
        Test(
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (nuint n, out nuint result) => MissingUIntPtrMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}