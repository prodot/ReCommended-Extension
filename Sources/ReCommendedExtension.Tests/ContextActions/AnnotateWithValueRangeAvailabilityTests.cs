using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    [TestNetCore30(ANNOTATIONS_PACKAGE)]
    public sealed class AnnotateWithValueRangeAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithValueRange>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithValueRange";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}