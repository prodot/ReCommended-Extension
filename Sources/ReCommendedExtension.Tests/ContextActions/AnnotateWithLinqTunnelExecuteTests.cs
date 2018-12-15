using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithLinqTunnelExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithLinqTunnel>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}