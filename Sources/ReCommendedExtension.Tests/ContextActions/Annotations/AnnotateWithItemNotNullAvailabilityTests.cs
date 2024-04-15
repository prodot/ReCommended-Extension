using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithItemNotNullAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithItemNotNull>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp73)]
    [TestNetCore20(ANNOTATIONS_PACKAGE, VALUE_TASKS_PACKAGE)]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    [NullableContext(NullableContextKind.Enable)]
    public void TestAvailabilityNullableAnnotationContext() => DoNamedTest2();
}