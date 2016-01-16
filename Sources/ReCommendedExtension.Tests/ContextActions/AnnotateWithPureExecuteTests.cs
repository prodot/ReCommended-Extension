using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithPureExecuteTests : ContextActionExecuteTestBase<AnnotateWithPure>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithPure";

        [Test]
        public void TestExecuteNotAnnotatedMethod() => DoNamedTest2();

        [Test]
        public void TestExecuteAnnotatedMethod() => DoNamedTest2();
    }
}