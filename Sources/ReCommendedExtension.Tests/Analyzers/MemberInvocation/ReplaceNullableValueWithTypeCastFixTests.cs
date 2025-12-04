using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class ReplaceNullableValueWithTypeCastFixTests : QuickFixTestBase<ReplaceNullableValueWithTypeCastSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestReplaceNullableValueWithTypeCastFix() => DoNamedTest2();

    [Test]
    public void TestReplaceNullableValueWithTypeCastFix_Parenthesized() => DoNamedTest2();
}