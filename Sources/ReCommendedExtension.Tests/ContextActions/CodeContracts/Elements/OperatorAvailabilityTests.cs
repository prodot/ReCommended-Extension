using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class OperatorAvailabilityTests : ContextActionAvailabilityTestBase<NotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Operator";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}