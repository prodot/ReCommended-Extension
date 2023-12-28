using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[TestNetCore30]
[NullableContext(NullableContextKind.Enable)]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
public sealed class AnnotateWithNotNullWhenTrueAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithNotNullWhenTrue>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNotNullWhenTrue";

    [Test]
    public void TestAvailability() => DoNamedTest2();
}