using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class AnnotateWithItemNotNullAvailabilityTests : ContextActionAvailabilityTestBase<AnnotateWithItemNotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

        [Test]
        public void TestAvailability() => DoNamedTest2();
    }
}