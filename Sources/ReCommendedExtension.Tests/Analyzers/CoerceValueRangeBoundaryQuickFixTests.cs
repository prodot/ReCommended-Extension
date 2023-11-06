using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers;

[TestFixture]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class CoerceValueRangeBoundaryQuickFixTests : QuickFixTestBase<CoerceValueRangeBoundary>
{
    protected override string RelativeTestDataPath => @"Analyzers\AnnotationQuickFixes";

    [Test]
    public void TestCoerceLowerBoundaryForSignedType() => DoNamedTest2();

    [Test]
    public void TestCoerceLowerBoundaryForUnsignedType() => DoNamedTest2();

    [Test]
    public void TestCoerceHigherBoundary() => DoNamedTest2();
}