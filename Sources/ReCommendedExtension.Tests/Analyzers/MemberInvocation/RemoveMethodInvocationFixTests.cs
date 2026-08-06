using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RedundantMethodInvocationHint.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void RemoveMethodInvocation_Expression() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RemoveMethodInvocation_Expression_Chained() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RemoveMethodInvocation_Statement() => DoNamedTest();

    [Test]
    [TestNetCore21]
    public void RemoveMethodInvocation_Statement_Chained() => DoNamedTest();
}