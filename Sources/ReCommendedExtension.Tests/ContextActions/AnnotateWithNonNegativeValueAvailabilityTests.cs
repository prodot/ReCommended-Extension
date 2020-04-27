using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public sealed class AnnotateWithNonNegativeValueAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithNonNegativeValue>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNonNegativeValue";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}