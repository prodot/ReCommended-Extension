using JetBrains.ProjectModel.Properties.CSharp;
using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[TestNetCore30]
public sealed class UseRangeIndexerFixTests : QuickFixTestBase<UseRangeIndexerSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseRangeIndexerFix() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseRangeIndexerFix_Nullable() => DoNamedTest2();

    [Test]
    public void TestUseRangeIndexerFix_Parenthesized_LeftOperand() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseRangeIndexerFix_Parenthesized_LeftOperand_Nullable() => DoNamedTest2();

    [Test]
    public void TestUseRangeIndexerFix_Parenthesized_RightOperand() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseRangeIndexerFix_Parenthesized_RightOperand_Nullable() => DoNamedTest2();

    [Test]
    public void TestUseRangeIndexerFix_Linq() => DoNamedTest2();

    [Test]
    [NullableContext(NullableContextKind.Enable)]
    public void TestUseRangeIndexerFix_Linq_Nullable() => DoNamedTest2();
}