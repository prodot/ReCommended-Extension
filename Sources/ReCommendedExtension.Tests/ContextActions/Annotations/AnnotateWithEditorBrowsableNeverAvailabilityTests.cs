using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
public sealed class AnnotateWithEditorBrowsableNeverAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithEditorBrowsableNever>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithEditorBrowsableNever";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}