using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AnnotateWithMustUseReturnValueAvailabilityTests : ContextActionAvailabilityTestBase<AnnotateWithMustUseReturnValue>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustUseReturnValue";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}