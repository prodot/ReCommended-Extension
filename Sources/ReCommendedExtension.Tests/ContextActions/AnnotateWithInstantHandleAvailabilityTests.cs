using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithInstantHandleAvailabilityTests : ContextActionAvailabilityTestBase<AnnotateWithInstantHandle>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}