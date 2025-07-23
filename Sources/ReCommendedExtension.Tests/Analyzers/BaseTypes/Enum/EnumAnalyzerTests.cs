using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Enum;

[TestFixture]
public sealed class EnumAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Enum";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is RedundantArgumentHint || highlighting.IsError();

    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    enum SampleEnum
    {
        Red,
        Green,
        Blue,
    }

    [Flags]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    enum SampleFlags
    {
        Red = 1 << 0,
        Green = 1 << 1,
        Blue = 1 << 2,
    }

    static readonly SampleEnum[] testValues = [SampleEnum.Red, (SampleEnum)1, (SampleEnum)10];
    static readonly SampleFlags[] testFlags = [SampleFlags.Red, SampleFlags.Red | SampleFlags.Blue, (SampleFlags)3, 0, (SampleFlags)9];

    static void TestEnum<R>(Func<SampleEnum, R> expected, Func<SampleEnum, R> actual)
    {
        foreach (var value in testValues)
        {
            Assert.AreEqual(expected(value), actual(value), $"with values: {value}");
        }
    }

    static void TestFlags<R>(Func<SampleFlags, R> expected, Func<SampleFlags, R> actual)
    {
        foreach (var value in testFlags)
        {
            Assert.AreEqual(expected(value), actual(value), $"with values: {value}");
        }
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void TestEnum<R>(FuncWithOut<SampleEnum, R, bool> expected, FuncWithOut<SampleEnum, R, bool> actual)
    {
        foreach (var value in testValues)
        {
            Assert.AreEqual(expected(value, out var expectedResult), actual(value, out var actualResult), $"with values: {value}");
            Assert.AreEqual(expectedResult, actualResult, $"with values: {value}");
        }
    }

    static void TestFlags<R>(FuncWithOut<SampleFlags, R, bool> expected, FuncWithOut<SampleFlags, R, bool> actual)
    {
        foreach (var value in testFlags)
        {
            Assert.AreEqual(expected(value, out var expectedResult), actual(value, out var actualResult), $"with values: {value}");
            Assert.AreEqual(expectedResult, actualResult, $"with values: {value}");
        }
    }

    [Test]
    [TestNet60]
    public void TestParse()
    {
        TestEnum(e => MissingEnumMethods.Parse<SampleEnum>(e.ToString(), false), e => MissingEnumMethods.Parse<SampleEnum>(e.ToString()));
        TestEnum(
            e => MissingEnumMethods.Parse<SampleEnum>(e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse<SampleEnum>(e.ToString().AsSpan()));
        TestEnum(e => System.Enum.Parse(typeof(SampleEnum), e.ToString(), false), e => System.Enum.Parse(typeof(SampleEnum), e.ToString()));
        TestEnum(
            e => MissingEnumMethods.Parse(typeof(SampleEnum), e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse(typeof(SampleEnum), e.ToString().AsSpan()));

        TestFlags(e => MissingEnumMethods.Parse<SampleFlags>(e.ToString(), false), e => MissingEnumMethods.Parse<SampleFlags>(e.ToString()));
        TestFlags(
            e => MissingEnumMethods.Parse<SampleFlags>(e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse<SampleFlags>(e.ToString().AsSpan()));
        TestFlags(e => System.Enum.Parse(typeof(SampleFlags), e.ToString(), false), e => System.Enum.Parse(typeof(SampleFlags), e.ToString()));
        TestFlags(
            e => MissingEnumMethods.Parse(typeof(SampleFlags), e.ToString().AsSpan(), false),
            e => MissingEnumMethods.Parse(typeof(SampleFlags), e.ToString().AsSpan()));

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        TestEnum(e => e.ToString(null as string), e => e.ToString());
        TestEnum(e => e.ToString(""), e => e.ToString());
        TestEnum(e => e.ToString("G"), e => e.ToString());
        TestEnum(e => e.ToString("g"), e => e.ToString());

        TestFlags(e => e.ToString(null as string), e => e.ToString());
        TestFlags(e => e.ToString(""), e => e.ToString());
        TestFlags(e => e.ToString("G"), e => e.ToString());
        TestFlags(e => e.ToString("g"), e => e.ToString());

        DoNamedTest2();
    }

    [Test]
    [TestNet60]
    public void TestTryParse()
    {
        TestEnum(
            (SampleEnum value, out SampleEnum result) => System.Enum.TryParse($"{value}", false, out result),
            (SampleEnum value, out SampleEnum result) => System.Enum.TryParse($"{value}", out result));
        TestEnum(
            (SampleEnum value, out SampleEnum result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), false, out result),
            (SampleEnum value, out SampleEnum result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), out result));
        TestEnum(
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}", false, out result),
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}", out result));
        TestEnum(
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), false, out result),
            (SampleEnum value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleEnum), $"{value}".AsSpan(), out result));

        TestFlags(
            (SampleFlags value, out SampleFlags result) => System.Enum.TryParse($"{value}", false, out result),
            (SampleFlags value, out SampleFlags result) => System.Enum.TryParse($"{value}", out result));
        TestFlags(
            (SampleFlags value, out SampleFlags result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), false, out result),
            (SampleFlags value, out SampleFlags result) => MissingEnumMethods.TryParse($"{value}".AsSpan(), out result));
        TestFlags(
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}", false, out result),
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}", out result));
        TestFlags(
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), false, out result),
            (SampleFlags value, out object? result) => MissingEnumMethods.TryParse(typeof(SampleFlags), $"{value}".AsSpan(), out result));

        DoNamedTest2();
    }
}