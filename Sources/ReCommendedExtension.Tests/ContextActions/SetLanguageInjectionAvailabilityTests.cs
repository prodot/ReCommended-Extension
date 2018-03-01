using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class SetLanguageInjectionAvailabilityTests : ContextActionAvailabilityTestBase<SetLanguageInjection>
    {
        protected override string RelativeTestDataPath => @"ContextActions\SetLanguageInjection";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}