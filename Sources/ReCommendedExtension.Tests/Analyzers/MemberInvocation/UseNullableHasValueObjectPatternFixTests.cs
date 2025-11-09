using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp70)]
public sealed class UseNullableHasValueObjectPatternFixTests : QuickFixTestBase<UseNullableHasValueObjectPatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseNullableHasValueObjectPatternFix() => DoNamedTest2();

    [Test]
    public void TestUseNullableHasValueObjectPatternFix_Parenthesized() => DoNamedTest2();
}