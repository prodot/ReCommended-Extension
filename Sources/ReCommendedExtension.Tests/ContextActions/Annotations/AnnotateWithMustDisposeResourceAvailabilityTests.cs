using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Projects;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithMustDisposeResourceAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithMustDisposeResource>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustDisposeResource";

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestAvailabilityTypes_Legacy() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityTypes() => DoNamedTest2();

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestAvailabilityConstructors_Legacy() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityConstructors() => DoNamedTest2();

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void TestAvailabilityPrimaryConstructors_Legacy() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityPrimaryConstructors() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityMethods() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityLocalFunctions() => DoNamedTest2();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityParameters() => DoNamedTest2();
}