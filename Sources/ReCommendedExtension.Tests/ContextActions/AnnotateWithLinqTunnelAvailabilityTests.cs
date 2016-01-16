using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithLinqTunnelAvailabilityTests : ContextActionAvailabilityTestBase<AnnotateWithLinqTunnel>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithLinqTunnel";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}