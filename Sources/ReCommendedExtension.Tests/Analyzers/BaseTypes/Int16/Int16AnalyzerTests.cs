using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int16;

[TestFixture]
public sealed class Int16AnalyzerTests : BaseTypeAnalyzerTests<short>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override short[] TestValues { get; } = [0, 1, 2, -1, -2, short.MinValue, short.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingInt16Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingInt16Methods.Clamp(number, short.MinValue, short.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (short)1, (short)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, short.MinValue, short.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingInt16Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((short)0, (short)10), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "UseExpressionResult")]
    public void TestGetTypeCode()
    {
        Test(number => number.GetTypeCode(), _ => TypeCode.Int16);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingInt16Methods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingInt16Methods.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingInt16Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMaxMagnitude()
    {
        Test(n => MissingInt16Methods.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingInt16Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingInt16Methods.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => short.Parse($"{n}", NumberStyles.Integer), n => short.Parse($"{n}"));
        Test(n => short.Parse($"{n}", null), n => short.Parse($"{n}"));
        Test(
            (n, provider) => short.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => short.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => short.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => short.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingInt16Methods.Parse($"{n}".AsSpan(), null), n => MissingInt16Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingInt16Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingInt16Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingInt16Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (short n, IFormatProvider? provider, out short result) => short.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}", null, out result),
            (short n, out short result) => short.TryParse($"{n}", out result));

        Test(
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), null, out result),
            (short n, out short result) => MissingInt16Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (short n, IFormatProvider? provider, out short result) => MissingInt16Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (short n, IFormatProvider? provider, out short result)
                => MissingInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (short n, out short result) => MissingInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (short n, out short result) => MissingInt16Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}