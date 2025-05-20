using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;
using ReCommendedExtension.Tests.Missing;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Guid;

[TestFixture]
public sealed class GuidAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Guid";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseBinaryOperatorSuggestion or UseExpressionResultSuggestion or RedundantArgumentHint || highlighting.IsError();

    static void Test<R>(Func<System.Guid, R> expected, Func<System.Guid, R> actual)
    {
        Assert.AreEqual(expected(System.Guid.Empty), actual(System.Guid.Empty));

        var guid = new System.Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        Assert.AreEqual(expected(guid), actual(guid));
    }

    static void Test<R>(Func<System.Guid, System.Guid, R> expected, Func<System.Guid, System.Guid, R> actual)
    {
        var guid = new System.Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);

        Assert.AreEqual(expected(System.Guid.Empty, System.Guid.Empty), actual(System.Guid.Empty, System.Guid.Empty));
        Assert.AreEqual(expected(guid, guid), actual(guid, guid));
        Assert.AreEqual(expected(System.Guid.Empty, guid), actual(System.Guid.Empty, guid));
        Assert.AreEqual(expected(guid, System.Guid.Empty), actual(guid, System.Guid.Empty));
    }

    delegate R FuncWithOut<in T, O, out R>(T arg1, out O arg2);

    static void Test<R>(FuncWithOut<System.Guid, R, bool> expected, FuncWithOut<System.Guid, R, bool> actual)
    {
        Assert.AreEqual(expected(System.Guid.Empty, out var expectedResult), actual(System.Guid.Empty, out var actualResult));
        Assert.AreEqual(expectedResult, actualResult);

        var guid = new System.Guid([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16]);
        Assert.AreEqual(expected(guid, out expectedResult), actual(guid, out actualResult));
        Assert.AreEqual(expectedResult, actualResult);
    }

    [Test]
    public void TestEquals()
    {
        Test((guid, g) => guid.Equals(g), (guid, g) => guid == g);

        Test(guid => guid.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestParse()
    {
        Test(guid => MissingGuidMethods.Parse(guid.ToString(), CultureInfo.CurrentCulture), guid => System.Guid.Parse(guid.ToString()));
        Test(
            guid => MissingGuidMethods.Parse(guid.ToString().AsSpan(), CultureInfo.CurrentCulture),
            guid => MissingGuidMethods.Parse(guid.ToString().AsSpan()));

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test(guid => guid.ToString(null), guid => guid.ToString());
        Test(guid => guid.ToString(""), guid => guid.ToString());
        Test(guid => guid.ToString("D"), guid => guid.ToString());
        Test(guid => guid.ToString("d"), guid => guid.ToString());

        Test(guid => guid.ToString("N", CultureInfo.CurrentCulture), guid => guid.ToString("N"));

        DoNamedTest2();
    }

    [Test]
    [TestNet70]
    public void TestTryParse()
    {
        Test(
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}", CultureInfo.CurrentCulture, out result),
            (System.Guid value, out System.Guid result) => System.Guid.TryParse($"{value}", out result));

        Test(
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), CultureInfo.CurrentCulture, out result),
            (System.Guid value, out System.Guid result) => MissingGuidMethods.TryParse($"{value}".AsSpan(), out result));

        DoNamedTest2();
    }
}