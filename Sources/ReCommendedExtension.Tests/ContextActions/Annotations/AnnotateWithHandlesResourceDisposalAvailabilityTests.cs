using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithHandlesResourceDisposalAvailabilityTests
    : CSharpContextActionAvailabilityTestBase<AnnotateWithHandlesResourceDisposal>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithHandlesResourceDisposal";

    [Test]
    public void TestAvailabilityMethods() => DoNamedTest2();

    [Test]
    public void TestAvailabilityParameters() => DoNamedTest2();

    [Test]
    public void TestAvailabilityProperties() => DoNamedTest2();

    [Test]
    public void TestAvailabilityFields() => DoNamedTest2();
}