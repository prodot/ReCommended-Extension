using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class DeclareEqualityOperatorsAvailabilityTests : CSharpContextActionAvailabilityTestBase<DeclareEqualityOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareEqualityOperators";

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
    public void TestAvailability_CS10() => DoNamedTest2();

    [Test]
    [TestNet60]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestAvailability_NET_6() => DoNamedTest2();
}