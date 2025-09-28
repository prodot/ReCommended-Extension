using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt128;

using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;

[TestFixture]
[TestNet70]
public sealed class UInt128AnalyzerTests : BaseTypeAnalyzerTests<uint128>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override uint128[] TestValues { get; } = [0, 1, 2, uint128.MaxValue];

    [Test]
    public void TestClamp()
    {
        Test(number => uint128.Clamp(number, 1, 1), _ => 1);
        Test(number => uint128.Clamp(number, uint128.MinValue, uint128.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    public void TestDivRem()
    {
        Test(() => uint128.DivRem(0, 10), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test((number, obj) => number.Equals(obj), (number, obj) => number == obj, TestValues, TestValues);

        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestMax()
    {
        Test(n => uint128.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMin()
    {
        Test(n => uint128.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestParse()
    {
        Test(n => uint128.Parse($"{n}", NumberStyles.Integer), n => uint128.Parse($"{n}"));
        Test(n => uint128.Parse($"{n}", null), n => uint128.Parse($"{n}"));
        Test(
            (n, provider) => uint128.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => uint128.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => uint128.Parse($"{n}", NumberStyles.None, null), n => uint128.Parse($"{n}", NumberStyles.None));

        Test(n => MissingUInt128Methods.Parse($"{n}".AsSpan(), null), n => MissingUInt128Methods.Parse($"{n}".AsSpan()));

        Test(
            n => MissingUInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}"), null),
            n => MissingUInt128Methods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    public void TestRotateLeft()
    {
        Test(n => uint128.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateRight()
    {
        Test(n => uint128.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => uint128.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => uint128.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", null, out result),
            (uint128 n, out uint128 result) => uint128.TryParse($"{n}", out result));

        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse(
                $"{n}".AsSpan(),
                NumberStyles.Integer,
                provider,
                out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (uint128 n, IFormatProvider? provider, out uint128 result) => MissingUInt128Methods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                provider,
                out result),
            TestValues,
            FormatProviders);
        Test(
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (uint128 n, out uint128 result) => MissingUInt128Methods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}