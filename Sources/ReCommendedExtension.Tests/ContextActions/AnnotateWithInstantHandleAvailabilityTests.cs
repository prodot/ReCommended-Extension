using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithInstantHandleAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithInstantHandle>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

        [Test]
        public void TestAvailability() => DoNamedTest2();

        [Test]
        [TestNetCore30("JetBrains.Annotations")]
        public void TestAvailabilityAsyncEnumerable() => DoNamedTest2();
    }
}