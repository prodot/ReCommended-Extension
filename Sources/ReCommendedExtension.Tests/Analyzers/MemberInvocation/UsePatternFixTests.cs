using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp90)]
[TestNet70]
public sealed class UsePatternFixTests : QuickFixTestBase<UsePatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUsePatternFix() => DoNamedTest2();

    [Test]
    public void TestUsePatternFix_Parenthesized_Inner() => DoNamedTest2();

    [Test]
    public void TestUsePatternFix_Parenthesized_Outer() => DoNamedTest2();
}