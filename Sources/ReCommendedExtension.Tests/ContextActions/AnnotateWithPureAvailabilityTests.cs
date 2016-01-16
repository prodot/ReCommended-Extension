using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithPureAvailabilityTests : ContextActionAvailabilityTestBase<AnnotateWithPure>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithPure";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}