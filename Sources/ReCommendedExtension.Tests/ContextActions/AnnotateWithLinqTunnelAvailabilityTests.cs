using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
public sealed class AnnotateWithLinqTunnelAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithLinqTunnel>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

    [Test]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public void TestAvailabilityAsyncEnumerable() => DoNamedTest2();
}