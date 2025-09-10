using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseType;

namespace ReCommendedExtension.Tests.Analyzers.BaseType;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
public sealed class BaseTypeQuickFixRemoveTests : QuickFixTestBase<RemoveBaseTypeDeclarationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseType\QuickFixes";

    [Test]
    public void TestClass() => DoNamedTest2();

    [Test]
    public void TestClass_Interface() => DoNamedTest2();

    [Test]
    public void TestClass_Empty() => DoNamedTest2();

    [Test]
    public void TestClass_NonEmpty() => DoNamedTest2();

    [Test]
    public void TestClass_PrimaryConstructor() => DoNamedTest2();

    [Test]
    public void TestRecord_Interface() => DoNamedTest2();
}