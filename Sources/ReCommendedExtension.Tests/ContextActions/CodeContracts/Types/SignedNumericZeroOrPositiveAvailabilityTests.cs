using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class SignedNumericZeroOrPositiveAvailabilityTests : ContextActionAvailabilityTestBase<SignedNumericZeroOrPositive>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\SignedNumericZeroOrPositive";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}