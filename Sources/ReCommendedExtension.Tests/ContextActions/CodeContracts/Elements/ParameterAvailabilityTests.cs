using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Elements
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class ParameterAvailabilityTests : ContextActionAvailabilityTestBase<NotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Elements\Parameter";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}