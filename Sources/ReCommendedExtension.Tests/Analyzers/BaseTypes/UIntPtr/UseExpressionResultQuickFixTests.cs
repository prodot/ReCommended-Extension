using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.UIntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UseExpressionResultQuickFixTests : QuickFixTestBase<UseExpressionResultFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\UIntPtrQuickFixes";

    [Test]
    [TestNet70]
    public void TestClamp() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestDivRem() => DoNamedTest2();

    [Test]
    public void TestEquals_Object() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestMax() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestMin() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateLeft() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestRotateRight() => DoNamedTest2();
}