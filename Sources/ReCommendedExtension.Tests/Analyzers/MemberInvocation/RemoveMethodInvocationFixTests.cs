using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.MemberInvocation;

namespace ReCommendedExtension.Tests.Analyzers.MemberInvocation;

[TestFixture]
public sealed class RemoveMethodInvocationFixTests : QuickFixTestBase<RemoveMethodInvocationFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\MemberInvocation\QuickFixes";

    [Test]
    public void TestRemoveMethodInvocation_Expression() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRemoveMethodInvocation_Expression_Chained() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRemoveMethodInvocation_Statement() => DoNamedTest2();

    [Test]
    [TestNetCore21]
    public void TestRemoveMethodInvocation_Statement_Chained() => DoNamedTest2();
}