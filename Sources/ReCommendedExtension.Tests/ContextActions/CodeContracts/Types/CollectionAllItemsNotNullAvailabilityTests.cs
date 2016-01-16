using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class CollectionAllItemsNotNullAvailabilityTests : ContextActionAvailabilityTestBase<CollectionAllItemsNotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\CollectionAllItemsNotNull";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}