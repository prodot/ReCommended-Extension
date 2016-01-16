using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AsyncVoidQuickFixChangeTypeTests : QuickFixTestBase<ChangeToAsyncTaskFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\AsyncVoidQuickFixes";

        [Test]
        public void TestAsyncVoidMethod() => DoNamedTest2();
    }
}