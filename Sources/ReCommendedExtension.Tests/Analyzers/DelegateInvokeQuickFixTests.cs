using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class DelegateInvokeQuickFixTests : QuickFixTestBase<RemoveDelegateInvokeFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\DelegateInvokeQuickFixes";

        [Test]
        public void TestDelegateInvoke() => DoNamedTest2();

        [Test]
        public void TestDelegateInvoke2() => DoNamedTest2();
    }
}