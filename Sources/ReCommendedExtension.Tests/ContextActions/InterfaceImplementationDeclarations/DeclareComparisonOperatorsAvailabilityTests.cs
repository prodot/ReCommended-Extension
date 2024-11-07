using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.InterfaceImplementationDeclarations;

namespace ReCommendedExtension.Tests.ContextActions.InterfaceImplementationDeclarations;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
[NullableContext(NullableContextKind.Enable)]
[TestNet60]
public sealed class DeclareComparisonOperatorsAvailabilityTests : CSharpContextActionAvailabilityTestBase<DeclareComparisonOperators>
{
    protected override string ExtraPath => "";

    protected override string RelativeTestDataPath => @"ContextActions\DeclareComparisonOperators";

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    [TestNet70]
    public void TestAvailability() => DoNamedTest2();

    [Test]
    [TestNet70]
    public void TestAvailability_CS10() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
    public void TestAvailability_NET_6() => DoNamedTest2();
}