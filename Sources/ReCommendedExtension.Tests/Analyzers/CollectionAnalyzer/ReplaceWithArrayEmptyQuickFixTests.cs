using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.CollectionAnalyzer;

[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[TestNetFramework46]
[TestFixture]
public sealed class ReplaceWithArrayEmptyQuickFixTests : QuickFixTestBase<ReplaceWithArrayEmptyFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\CollectionQuickFixes";

    [Test]
    public void TestEmptyArrayInitialization() => DoNamedTest2();

    [Test]
    public void TestEmptyArrayInitialization2() => DoNamedTest2();

    [Test]
    public void TestEmptyArrayInitialization3() => DoNamedTest2();

    [Test]
    public void TestEmptyArrayInitialization4() => DoNamedTest2();
}