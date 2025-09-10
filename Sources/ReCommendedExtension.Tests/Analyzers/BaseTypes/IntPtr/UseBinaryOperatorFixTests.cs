using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseTypes;

namespace ReCommendedExtension.Tests.Analyzers.BaseTypes.IntPtr;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet50]
public sealed class UseBinaryOperatorFixTests : QuickFixTestBase<UseBinaryOperatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseTypes\IntPtr\QuickFixes";

    [Test]
    public void TestEquals_IntPtr() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsNegative() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestIsPositive() => DoNamedTest2();
}