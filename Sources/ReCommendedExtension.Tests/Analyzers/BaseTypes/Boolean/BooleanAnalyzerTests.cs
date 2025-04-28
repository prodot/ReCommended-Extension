using System.Globalization;
using JetBrains.Application.Settings;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.FeaturesTestFramework.Daemon;
using JetBrains.ReSharper.Psi;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.Boolean;

[TestFixture]
public sealed class BooleanAnalyzerTests : CSharpHighlightingTestBase
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\Boolean";

    protected override bool HighlightingPredicate(IHighlighting highlighting, IPsiSourceFile sourceFile, IContextBoundSettingsStore settingsStore)
        => highlighting is UseExpressionResultSuggestion or UseBinaryOperatorSuggestion or RedundantMethodInvocationHint or RedundantArgumentHint
            || highlighting.IsError();

    static void Test<R>(Func<bool, R> expected, Func<bool, R> actual)
    {
        Assert.AreEqual(expected(true), actual(true));
        Assert.AreEqual(expected(false), actual(false));
    }

    static void Test<R>(Func<bool, bool, R> expected, Func<bool, bool, R> actual)
    {
        Assert.AreEqual(expected(true, true), actual(true, true));
        Assert.AreEqual(expected(true, false), actual(true, false));
        Assert.AreEqual(expected(false, true), actual(false, true));
        Assert.AreEqual(expected(false, false), actual(false, false));
    }

    [Test]
    public void TestEquals()
    {
        Test(flag => flag.Equals(true), flag => flag);
        Test(flag => flag.Equals(false), flag => !flag);
        Test(obj => true.Equals(obj), obj => obj);
        Test(obj => false.Equals(obj), obj => !obj);
        Test((flag, obj) => flag.Equals(obj), (flag, obj) => flag == obj);

        Test(flag => flag.Equals(null), _ => false);

        DoNamedTest2();
    }

    [Test]
    public void TestGetTypeCode()
    {
        Test(flag => flag.GetTypeCode(), _ => TypeCode.Boolean);

        DoNamedTest2();
    }

    [Test]
    public void TestToString()
    {
        Test(flag => flag.ToString(null), flag => flag.ToString());
        Test(flag => flag.ToString(CultureInfo.CurrentCulture), flag => flag.ToString());

        DoNamedTest2();
    }
}