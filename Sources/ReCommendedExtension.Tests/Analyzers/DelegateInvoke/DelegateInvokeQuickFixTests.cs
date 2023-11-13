using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.DelegateInvoke;

namespace ReCommendedExtension.Tests.Analyzers.DelegateInvoke;

[TestFixture]
public sealed class DelegateInvokeQuickFixTests : QuickFixTestBase<RemoveDelegateInvokeFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\DelegateInvokeQuickFixes";

    [Test]
    public void TestDelegateInvoke() => DoNamedTest2();

    [Test]
    public void TestDelegateInvoke2() => DoNamedTest2();
}