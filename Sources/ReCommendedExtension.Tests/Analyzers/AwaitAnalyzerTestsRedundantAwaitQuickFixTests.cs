using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Await;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AwaitAnalyzerTestsRedundantAwaitQuickFixTests : QuickFixTestBase<RemoveAsyncAwaitFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\AwaitQuickFixes";

        [Test]
        public new void TestMethod() => DoNamedTest2();

        [Test]
        public void TestMethod_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestMethod_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestMethod_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestMethod2() => DoNamedTest2();

        [Test]
        public void TestMethod2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestMethod2_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestMethod2_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestMethod4() => DoNamedTest2();

        [Test]
        public void TestMethod4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestMethod4_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestMethod4_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable2() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable2_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable2_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable4() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable4_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaVariable4_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField() => DoNamedTest2();

        [Test]
        public void TestLambdaField_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaField_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField2() => DoNamedTest2();

        [Test]
        public void TestLambdaField2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField2_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaField2_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField4() => DoNamedTest2();

        [Test]
        public void TestLambdaField4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLambdaField4_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLambdaField4_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable2() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable4() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodVariable4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField2() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField4() => DoNamedTest2();

        [Test]
        public void TestAnonymousMethodField4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction() => DoNamedTest2();

        [Test]
        public void TestLocalFunction_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLocalFunction_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction2() => DoNamedTest2();

        [Test]
        public void TestLocalFunction2_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction2_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLocalFunction2_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction4() => DoNamedTest2();

        [Test]
        public void TestLocalFunction4_WithConfigureAwait() => DoNamedTest2();

        [Test]
        public void TestLocalFunction4_AsExpressionBodied() => DoNamedTest2();

        [Test]
        public void TestLocalFunction4_AsExpressionBodied_WithConfigureAwait() => DoNamedTest2();
    }
}