using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AsyncVoidQuickFixRemoveTests : QuickFixTestBase<RemoveAsyncFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\AsyncVoidQuickFixes";

        [Test]
        public void TestAnonymousMethod() => DoNamedTest2();

        [Test]
        public void TestLambdaExpression() => DoNamedTest2();
    }
}