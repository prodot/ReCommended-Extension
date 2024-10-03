using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Linq;

namespace ReCommendedExtension.Tests.Analyzers.Linq;

public sealed class UsePropertyQuickFixTests : QuickFixTestBase<UsePropertyFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\LinqQuickFixes";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestUseProperty_Collection() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestUseProperty_Array() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
    [TestNet50]
    public void TestUseProperty_String() => DoNamedTest2();
}