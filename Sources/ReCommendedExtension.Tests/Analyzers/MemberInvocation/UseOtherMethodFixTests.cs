using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[TestNetCore21]
public sealed class UseOtherMethodFixTests : QuickFixTestBase<UseOtherMethodFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseOtherMethodFix_LeftOperand() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseOtherMethodFix_LeftOperand_Negated() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseOtherMethodFix_RightOperand() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseOtherMethodFix_RightOperand_Negated() => DoNamedTest2();

    [Test]
    [CSharpLanguageLevel(CSharpLanguageLevel.CSharp120)]
    public void TestUseOtherMethodFix_Standalone() => DoNamedTest2();
}