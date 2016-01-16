using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class RemoveAssertNotNullExecuteTests : ContextActionExecuteTestBase<RemoveAssertNotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\RemoveAssertNotNull";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}