using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Int128;

using int128 = ReCommendedExtension.Analyzers.BaseTypes.NumberInfos.Int128;

[TestFixture]
[TestNet70]
public sealed class Int128AnalyzerTests : BaseTypeAnalyzerTests<int128>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Int128";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion || highlighting.IsError();

    protected override int128[] TestValues { get; } = [0, 1, 2, -1, -2, int128.MinValue, int128.MaxValue];

    [Test]
    public void TestClamp()
    {
        Test(number => int128.Clamp(number, 1, 1), _ => 1);
        Test(number => int128.Clamp(number, int128.MinValue, int128.MaxValue), number => number);

        DoNamedTest2();
    }

    [Test]
    public void TestDivRem()
    {
        Test(() => int128.DivRem(0, 10), () => (0, 0));

        DoNamedTest2();
    }

    [Test]
    public void TestEquals()
    {
        Test(number => number.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestMax()
    {
        Test(n => int128.Max(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMaxMagnitude()
    {
        Test(n => int128.MaxMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMin()
    {
        Test(n => int128.Min(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestMinMagnitude()
    {
        Test(n => int128.MinMagnitude(n, n), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateLeft()
    {
        Test(n => int128.RotateLeft(n, 0), n => n);

        DoNamedTest2();
    }

    [Test]
    public void TestRotateRight()
    {
        Test(n => int128.RotateRight(n, 0), n => n);

        DoNamedTest2();
    }
}