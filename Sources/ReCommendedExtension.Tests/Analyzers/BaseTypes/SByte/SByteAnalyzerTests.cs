using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.SByte;

[TestFixture]
public sealed class SByteAnalyzerTests : BaseTypeAnalyzerTests<sbyte>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\SByte";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override sbyte[] TestValues { get; } = [0, 1, 2, -1, -2, sbyte.MinValue, sbyte.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingSByteMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingSByteMethods.Clamp(number, sbyte.MinValue, sbyte.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (sbyte)1, (sbyte)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, sbyte.MinValue, sbyte.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingSByteMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((sbyte)0, (sbyte)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.SByte);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsNegative()
    {
        Test(number => MissingSByteMethods.IsNegative(number), number => number < 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    public void TestIsPositive()
    {
        Test(number => MissingSByteMethods.IsPositive(number), number => number >= 0);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingSByteMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMaxMagnitude()
    {
        Test(n => MissingSByteMethods.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingSByteMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMinMagnitude()
    {
        Test(n => MissingSByteMethods.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => sbyte.Parse($"{n}", NumberStyles.Integer), n => sbyte.Parse($"{n}"));
        Test(n => sbyte.Parse($"{n}", null), n => sbyte.Parse($"{n}"));
        Test(
            (n, provider) => sbyte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => sbyte.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => sbyte.Parse($"{n}", NumberStyles.AllowLeadingSign, null), n => sbyte.Parse($"{n}", NumberStyles.AllowLeadingSign));

        Test(n => MissingSByteMethods.Parse($"{n}".AsSpan(), null), n => MissingSByteMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingSByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingSByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingSByteMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingSByteMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result) => sbyte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}", null, out result),
            (sbyte n, out sbyte result) => sbyte.TryParse($"{n}", out result));

        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (sbyte n, IFormatProvider? provider, out sbyte result) => MissingSByteMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (sbyte n, IFormatProvider? provider, out sbyte result)
                => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (sbyte n, out sbyte result) => MissingSByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}