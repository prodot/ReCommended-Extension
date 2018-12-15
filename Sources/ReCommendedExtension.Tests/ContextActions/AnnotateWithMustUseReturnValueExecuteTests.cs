using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AnnotateWithMustUseReturnValueExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithMustUseReturnValue>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithMustUseReturnValue";

        [Test]
        public void TestExecuteNonPureMethod() => DoNamedTest2();

        [Test]
        public void TestExecutePureMethod() => DoNamedTest2();
    }
}