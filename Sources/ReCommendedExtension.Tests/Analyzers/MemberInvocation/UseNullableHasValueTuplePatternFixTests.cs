using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
[TestNetCore20]
public sealed class UseNullableHasValueTuplePatternFixTests : QuickFixTestBase<UseNullableHasValueTuplePatternFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestUseNullableHasValueTuplePatternFix() => DoNamedTest2();

    [Test]
    public void TestUseNullableHasValueTuplePatternFix_Parenthesized() => DoNamedTest2();
}