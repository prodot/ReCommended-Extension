using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestFixture]
    public sealed class AnnotateWithNonNegativeValueExecuteTests : CSharpContextActionExecuteTestBase<AnnotateWithNonNegativeValue>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithNonNegativeValue";

        [Test]
        [TestNetCore30(ANNOTATIONS_PACKAGE)]
        public void TestExecute() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
        [TestNet60(ANNOTATIONS_PACKAGE)]
        public void TestExecuteLambda() => DoNamedTest2();

        [Test]
        [CSharpLanguageLevel(CSharpLanguageLevel.CSharp100)]
        [TestNet60(ANNOTATIONS_PACKAGE)]
        public void TestExecuteLambda2() => DoNamedTest2();
    }
}