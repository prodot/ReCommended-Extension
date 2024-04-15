using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[TestNet80(ANNOTATIONS_PACKAGE)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
public sealed class AnnotateWithMustUseReturnValueAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithMustUseReturnValue>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustUseReturnValue";

    [Test]
    public void TestAvailabilityMethods() => DoNamedTest2();

    [Test]
    public void TestAvailabilityLocalFunctions() => DoNamedTest2();
}