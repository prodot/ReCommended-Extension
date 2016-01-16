using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class TimeSpanNegativeAvailabilityTests : ContextActionAvailabilityTestBase<TimeSpanNegative>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\TimeSpanNegative";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}