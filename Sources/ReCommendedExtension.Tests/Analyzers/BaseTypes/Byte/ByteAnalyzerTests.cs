﻿using System.Globalization;
using System.Text;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Byte;

[TestFixture]
public sealed class ByteAnalyzerTests : BaseTypeAnalyzerTests<byte>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Byte";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantArgumentHint || highlighting.IsError();

    protected override byte[] TestValues { get; } = [0, 1, 2, byte.MaxValue];

    [Test]
    [TestNet70]
    public void TestClamp()
    {
        Test(number => MissingByteMethods.Clamp(number, 1, 1), _ => 1);
        Test(number => MissingByteMethods.Clamp(number, 0, 255), number => number);

        Test(number => MissingMathMethods.Clamp(number, (byte)1, (byte)1), _ => 1);
        Test(number => MissingMathMethods.Clamp(number, byte.MinValue, byte.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestDivRem()
    {
        Test(() => MissingByteMethods.DivRem(0, 10), () => (0, 0));

        Test(() => MissingMathMethods.DivRem((byte)0, (byte)10), () => (0, 0));

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
        Test(number => number.GetTypeCode(), _ => TypeCode.Byte);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMax()
    {
        Test(n => MissingByteMethods.Max(n, n), n => n);
        Test(n => Math.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestMin()
    {
        Test(n => MissingByteMethods.Min(n, n), n => n);
        Test(n => Math.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    [SuppressMessage("ReSharper", "RedundantArgument")]
    public void TestParse()
    {
        Test(n => byte.Parse($"{n}", NumberStyles.Integer), n => byte.Parse($"{n}"));
        Test(n => byte.Parse($"{n}", null), n => byte.Parse($"{n}"));
        Test(
            (n, provider) => byte.Parse($"{n}", NumberStyles.Integer, provider),
            (n, provider) => byte.Parse($"{n}", provider),
            TestValues,
            FormatProviders);
        Test(n => byte.Parse($"{n}", NumberStyles.None, null), n => byte.Parse($"{n}", NumberStyles.None));

        Test(n => MissingByteMethods.Parse($"{n}".AsSpan(), null), n => MissingByteMethods.Parse($"{n}".AsSpan()));

        Test(n => MissingByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}"), null), n => MissingByteMethods.Parse(Encoding.UTF8.GetBytes($"{n}")));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateLeft()
    {
        Test(n => MissingByteMethods.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestRotateRight()
    {
        Test(n => MissingByteMethods.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    [TestNet80]
    public void TestTryParse()
    {
        Test(
            (byte n, IFormatProvider? provider, out byte result) => byte.TryParse($"{n}", NumberStyles.Integer, provider, out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse($"{n}", provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}", null, out result),
            (byte n, out byte result) => byte.TryParse($"{n}", out result));

        Test(
            (byte n, IFormatProvider? provider, out byte result)
                => MissingByteMethods.TryParse($"{n}".AsSpan(), NumberStyles.Integer, provider, out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse($"{n}".AsSpan(), out result));

        Test(
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse(
                Encoding.UTF8.GetBytes($"{n}"),
                NumberStyles.Integer,
                provider,
                out result),
            (byte n, IFormatProvider? provider, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), provider, out result),
            TestValues,
            FormatProviders);
        Test(
            (byte n, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), null, out result),
            (byte n, out byte result) => MissingByteMethods.TryParse(Encoding.UTF8.GetBytes($"{n}"), out result));

        DoNamedTest2();
    }
}