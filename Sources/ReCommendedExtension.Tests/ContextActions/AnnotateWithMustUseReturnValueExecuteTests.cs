using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AnnotateWithMustUseReturnValueExecuteTests : ContextActionExecuteTestBase<AnnotateWithMustUseReturnValue>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustUseReturnValue";

        [Test]
        public void TestExecuteNonPureMethod() => DoNamedTest2();

        [Test]
        public void TestExecutePureMethod() => DoNamedTest2();
    }
}