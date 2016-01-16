using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class RemoveAssertNotNullAvailabilityTests : ContextActionAvailabilityTestBase<RemoveAssertNotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\RemoveAssertNotNull";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}