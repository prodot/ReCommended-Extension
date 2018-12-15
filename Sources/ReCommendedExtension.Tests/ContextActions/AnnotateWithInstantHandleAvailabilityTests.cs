using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
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
    }
}