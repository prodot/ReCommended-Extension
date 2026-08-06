using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.Annotations;

namespace ReCommendedExtension.Tests.ContextActions.Annotations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
public sealed class AnnotateWithLinqTunnelAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithLinqTunnel>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

    [Test]
    public void Availability() => DoNamedTest();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void AvailabilityAsyncEnumerable() => DoNamedTest();
}