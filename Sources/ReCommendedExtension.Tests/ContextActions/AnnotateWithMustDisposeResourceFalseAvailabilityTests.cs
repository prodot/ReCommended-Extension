using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithMustDisposeResourceFalseAvailabilityTests
    : CSharpContextActionAvailabilityTestBase<AnnotateWithMustDisposeResourceFalse>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustDisposeResourceFalse";

    [Test]
    public void TestAvailabilityTypes() => DoNamedTest2();

    [Test]
    public void TestAvailabilityConstructors() => DoNamedTest2();

    [Test]
    public void TestAvailabilityPrimaryConstructors() => DoNamedTest2();

    [Test]
    public void TestAvailabilityMethods() => DoNamedTest2();

    [Test]
    public void TestAvailabilityLocalFunctions() => DoNamedTest2();

    [Test]
    public void TestAvailabilityParameters() => DoNamedTest2();
}