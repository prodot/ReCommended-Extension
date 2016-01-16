using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class FieldAvailabilityTests : ContextActionAvailabilityTestBase<NotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Field";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}