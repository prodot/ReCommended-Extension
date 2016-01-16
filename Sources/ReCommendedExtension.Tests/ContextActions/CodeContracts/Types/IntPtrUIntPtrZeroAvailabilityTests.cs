using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class IntPtrUIntPtrZeroAvailabilityTests : ContextActionAvailabilityTestBase<IntPtrUIntPtrZero>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\IntPtrUIntPtrZero";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}