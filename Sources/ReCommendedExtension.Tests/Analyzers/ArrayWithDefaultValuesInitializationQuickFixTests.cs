using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ArrayWithDefaultValuesInitializationQuickFixTests : QuickFixTestBase<ArrayWithDefaultValuesInitializationFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\ArrayWithDefaultValuesInitializationQuickFixes";

        [Test]
        public void TestArrayWithDefaultValuesInitialization() => DoNamedTest2();

        [Test]
        public void TestArrayWithDefaultValuesInitialization2() => DoNamedTest2();

        [Test]
        public void TestArrayWithDefaultValuesInitialization3() => DoNamedTest2();

        [Test]
        public void TestArrayWithDefaultValuesInitialization4() => DoNamedTest2();
    }
}