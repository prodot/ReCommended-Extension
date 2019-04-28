using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.ControlFlow;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class ControlFlowQuickFixTests : QuickFixTestBase<RemoveAssertionStatementFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\ControlFlowQuickFixes";

        [Test]
        public void TestControlFlow() => DoNamedTest2();
    }
}