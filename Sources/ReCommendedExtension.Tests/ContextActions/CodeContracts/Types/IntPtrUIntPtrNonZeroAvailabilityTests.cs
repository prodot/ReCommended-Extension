using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class IntPtrUIntPtrNonZeroAvailabilityTests : ContextActionAvailabilityTestBase<IntPtrUIntPtrNonZero>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\IntPtrUIntPtrNonZero";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}