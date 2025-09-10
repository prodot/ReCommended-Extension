using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[TestNetFramework45]
[TestPackages(ANNOTATIONS_PACKAGE)]
public sealed class AnnotationQuickFixTests : QuickFixTestBase<RemoveAttributeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation\QuickFixes";

    [Test]
    public void TestNotAllowed() => DoNamedTest2();

    [Test]
    public void TestConflicting() => DoNamedTest2();

    [Test]
    public void TestRedundant() => DoNamedTest2();
}