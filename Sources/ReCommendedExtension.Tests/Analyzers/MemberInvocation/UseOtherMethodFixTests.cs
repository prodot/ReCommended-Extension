using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[TestNetCore21]
public sealed class UseOtherMethodFixTests : QuickFixTestBase<UseOtherMethodSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UseOtherMethodFix_LeftOperand() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UseOtherMethodFix_LeftOperand_Negated() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UseOtherMethodFix_RightOperand() => DoNamedTest();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void UseOtherMethodFix_RightOperand_Negated() => DoNamedTest();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void UseOtherMethodFix_Standalone() => DoNamedTest();
}