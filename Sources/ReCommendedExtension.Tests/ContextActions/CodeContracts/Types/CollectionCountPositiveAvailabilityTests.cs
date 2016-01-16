using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class CollectionCountPositiveAvailabilityTests : ContextActionAvailabilityTestBase<CollectionCountPositive>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\CollectionCountPositive";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}