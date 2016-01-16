using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AddConfigureAwaitAvailabilityTests : ContextActionAvailabilityTestBase<AddConfigureAwait>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AddConfigureAwait";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}