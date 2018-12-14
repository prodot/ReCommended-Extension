using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestPackagesWithAnnotations("System.Threading.Tasks.Extensions")]
    [TestFixture]
    public sealed class AnnotateWithItemNotNullAvailabilityTests : CSharpContextActionAvailabilityTestBase<AnnotateWithItemNotNull>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}