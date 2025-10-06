using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UInt16;

[TestFixture]
public sealed class UInt16AnalyzerTests : BaseTypeAnalyzerTests<ushort>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UInt16";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion || highlighting.IsError();

    protected override ushort[] TestValues { get; } = [0, 1, 2, ushort.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingUInt16Methods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingUInt16Methods.Clamp(number, ushort.MinValue, ushort.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, (ushort)1, (ushort)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, ushort.MinValue, ushort.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingUInt16Methods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((ushort)0, (ushort)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.UInt16);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingUInt16Methods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingUInt16Methods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingUInt16Methods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingUInt16Methods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }
}