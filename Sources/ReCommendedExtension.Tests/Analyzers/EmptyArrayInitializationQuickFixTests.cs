using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework46]
    [TestFixture]
    public sealed class EmptyArrayInitializationQuickFixTests : QuickFixTestBase<EmptyArrayInitializationFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\EmptyArrayInitializationQuickFixes";

        [Test]
        public void TestEmptyArrayInitialization() => DoNamedTest2();

        [Test]
        public void TestEmptyArrayInitialization2() => DoNamedTest2();

        [Test]
        public void TestEmptyArrayInitialization3() => DoNamedTest2();
    }
}