using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class InternalConstructorQuickFixTests : QuickFixTestBase<ChangeConstructorVisibilityFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\InternalConstructorQuickFixes";

        [Test]
        public void TestInternalConstructorToProtected() => DoNamedTest2();

        [Test]
        public void TestInternalConstructorToPrivateProtected() => DoNamedTest2();
    }
}