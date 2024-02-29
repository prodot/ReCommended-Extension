using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.InterfaceImplementationDeclarations;

namespace ReCommendedExtension.Tests.ContextActions.InterfaceImplementationDeclarations;

[TestFixture]
[NullableContext(NullableContextKind.Enable)]
public sealed class DeclareComparisonOperatorsAvailabilityTests : CSharpContextActionAvailabilityTestBase<DeclareComparisonOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareComparisonOperators";

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