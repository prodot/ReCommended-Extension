using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithLinqTunnelExecuteTests : ContextActionExecuteTestBase<AnnotateWithLinqTunnel>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}