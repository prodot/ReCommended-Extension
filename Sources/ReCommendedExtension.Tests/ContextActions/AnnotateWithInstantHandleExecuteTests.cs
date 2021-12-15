using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithInstantHandleExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithInstantHandle>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithInstantHandle";

        [Test]
        public void TestExecuteGenericEnumerable() => DoNamedTest2();

        [Test]
        public void TestExecuteDelegate() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp80)]
        public void TestExecuteAsyncEnumerable() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
        public void TestExecuteLambda() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
        public void TestExecuteLambda2() => DoNamedTest2();
    }
}