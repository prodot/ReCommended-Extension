using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.DocComments;

namespace ReCommendedExtension.Tests.ContextActions.DocComments;

[TestFixture]
public sealed class ReflowDocCommentsAvailabilityTests : CSharpContextActionAvailabilityTestBase<ReflowDocComments>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\ReflowDocComments";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void Availability_TopLevelTags() => DoNamedTest();

    [Test]
    public void Availability_NestedTags() => DoNamedTest();
}