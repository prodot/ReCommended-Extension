using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt32;

[TestFixture]
public sealed class UInt32AnalyzerTests : BaseTypeAnalyzerTests<uint>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt32";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override uint[] TestValues { get; } = [0, 1, 2, uint.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt32Methods.Clamp(number, 1, 1), _ => 1u);
        Test(number => MissingUInt32Methods.Clamp(number, uint.MinValue, uint.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1u, 1u), _ => 1u);
        Test(number => MissingMathMethods.Clamp(number, uint.MinValue, uint.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt32Methods.DivRem(0, 10), () => (0u, 0u));

        Test(() => MissingMathMethods.DivRem(0u, 10u), () => (0u, 0u));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt32);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt32Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt32Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => uint.Parse($"{n}", NumberStyles.Integer), n => uint.Parse($"{n}"));
        Test(n => uint.Parse($"{n}", null), n => uint.Parse($"{n}"));
        Test(
            (n, provider) => uint.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => uint.Parse($"{n}", NumberStyles.None, null), n => uint.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt32Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt32Methods.Parse($"{n}".AsSpan()));

        Test(n => MissingUInt32Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingUInt32Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt32Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt32Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (uint n, IFormatProvider? provider, out uint result) => uint.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}", null, out result),
            (uint n, out uint result) => uint.TryParse($"{n}", out result));

        Test(
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), null, out result),
            (uint n, out uint result) => MissingUInt32Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (uint n, IFormatProvider? provider, out uint result) => MissingUInt32Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (uint n, IFormatProvider? provider, out uint result)
                => MissingUInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (uint n, out uint result) => MissingUInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (uint n, out uint result) => MissingUInt32Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}