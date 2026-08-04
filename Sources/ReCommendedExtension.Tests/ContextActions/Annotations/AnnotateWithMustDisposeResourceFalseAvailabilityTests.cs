using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework.Projects;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithMustDisposeResourceFalseAvailabilityTests
    : CSharpContextActionAvailabilityTestBase<AnnotateWithMustDisposeResourceFalse>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustDisposeResourceFalse";

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void AvailabilityTypes_Legacy() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityTypes() => DoNamedTest();

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void AvailabilityConstructors_Legacy() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityConstructors() => DoNamedTest();

    [Test]
    [TestNet80("JetBrains.Annotations/2023.3.0")] // structs cannot be annotated with [MustDisposeResource]
    [ReuseSolution(false)] // prevents reusing cached packages
    public void AvailabilityPrimaryConstructors_Legacy() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityPrimaryConstructors() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityMethods() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityLocalFunctions() => DoNamedTest();

    [Test]
    [TestNet80(ANNOTATIONS_PACKAGE)]
    public void AvailabilityParameters() => DoNamedTest();
}