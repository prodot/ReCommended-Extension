using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class UseNullableHasValueNotNullFixTests : QuickFixTestBase<UseNullableHasValueAlternativeSuggestion.NotNullFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void UseNullableHasValueNotNullFix() => DoNamedTest();

    [Test]
    public void UseNullableHasValueNotNullFix_Parenthesized() => DoNamedTest();
}