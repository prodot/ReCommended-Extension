using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class ReplaceNullableValueWithTypeCastFixTests : QuickFixTestBase<ReplaceNullableValueWithTypeCastSuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void ReplaceNullableValueWithTypeCastFix() => DoNamedTest();

    [Test]
    public void ReplaceNullableValueWithTypeCastFix_Parenthesized() => DoNamedTest();
}