using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithInstantHandleExecuteTests : ContextActionExecuteTestBase<AnnotateWithInstantHandle>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

        [Test]
        public void TestExecuteGenericEnumerable() => DoNamedTest2();

        [Test]
        public void TestExecuteDelegate() => DoNamedTest2();
    }
}