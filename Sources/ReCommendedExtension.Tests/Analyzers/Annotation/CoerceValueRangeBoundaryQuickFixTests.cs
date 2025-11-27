using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Annotation;

namespace ReCommendedExtension.Tests.Analyzers.Annotation;

[TestFixture]
[TestNetCore30(ANNOTATIONS_PACKAGE)]
public sealed class CoerceValueRangeBoundaryQuickFixTests : QuickFixTestBase<InvalidValueRangeBoundaryWarning.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\Annotation\QuickFixes";

    [Test]
    public void TestCoerceLowerBoundaryForSignedType() => DoNamedTest2();

    [Test]
    public void TestCoerceLowerBoundaryForUnsignedType() => DoNamedTest2();

    [Test]
    public void TestCoerceHigherBoundary() => DoNamedTest2();
}