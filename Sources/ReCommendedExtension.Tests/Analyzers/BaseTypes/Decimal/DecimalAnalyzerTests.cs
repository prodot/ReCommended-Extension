using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Decimal;

[TestFixture]
public sealed class DecimalAnalyzerTests : BaseTypeAnalyzerTests<decimal>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Decimal";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or UseUnaryOperatorSuggestion || highlighting.IsError();

    protected override decimal[] TestValues { get; } = [0, -0.0m, 1, 2, -1, -2, 1.2m, -1.2m, decimal.MinValue, decimal.MaxValue];

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestAdd()
    {
        Test(
            (d1, d2) => decimal.Add(d1, d2),
            (d1, d2) => d1 + d2,
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])],
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingDecimalMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingDecimalMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

        Test(number => MissingMathMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, decimal.MinValue, decimal.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestDivide()
    {
        Test((d1, d2) => decimal.Divide(d1, d2), (d1, d2) => d1 / d2, [..TestValues.Except([decimal.MinValue, decimal.MaxValue])], [1, -1, 2, -2]);

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Decimal);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingDecimalMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingDecimalMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestMultiply()
    {
        Test(
            (d1, d2) => decimal.Multiply(d1, d2),
            (d1, d2) => d1 * d2,
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])],
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseUnaryOperator")]
    public void TestNegate()
    {
        Test(d => decimal.Negate(d), d => -d);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestRemainder()
    {
        Test(
            (d1, d2) => decimal.Remainder(d1, d2),
            (d1, d2) => d1 % d2,
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])],
            [1, -1, 2, -2]);

        DoNamedTest2();
    }

    [Test]
    [SuppressMessage("ReSharper", "ConvertClosureToMethodGroup")]
    [SuppressMessage("ReSharper", "UseBinaryOperator")]
    public void TestSubtract()
    {
        Test(
            (d1, d2) => decimal.Subtract(d1, d2),
            (d1, d2) => d1 - d2,
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])],
            [..TestValues.Except([decimal.MinValue, decimal.MaxValue])]);

        DoNamedTest2();
    }
}