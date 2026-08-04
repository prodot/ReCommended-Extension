using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.BaseType;

namespace ReCommendedExtension.Tests.Analyzers.BaseType;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
[TestNet80]
public sealed class BaseTypeQuickFixRemoveTests : QuickFixTestBase<RemoveRedundantBaseTypeDeclarationHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\BaseType\QuickFixes";

    [Test]
    public void Class() => DoNamedTest();

    [Test]
    public void Class_Interface() => DoNamedTest();

    [Test]
    public void Class_Empty() => DoNamedTest();

    [Test]
    public void Class_NonEmpty() => DoNamedTest();

    [Test]
    public void Class_PrimaryConstructor() => DoNamedTest();

    [Test]
    public void Record_Interface() => DoNamedTest();
}