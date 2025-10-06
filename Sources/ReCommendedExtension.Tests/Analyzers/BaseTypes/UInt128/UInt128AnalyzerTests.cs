using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt128;

using uint128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.UInt128;

[TestFixture]
[TestNet70]
public sealed class UInt128AnalyzerTests : BaseTypeAnalyzerTests<uint128>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion || highlighting.IsError();

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
}