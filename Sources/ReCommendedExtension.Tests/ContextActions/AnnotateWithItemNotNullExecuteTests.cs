using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class AnnotateWithItemNotNullExecuteTests : ContextActionExecuteTestBase<AnnotateWithItemNotNull>
    {
        protected override string RelativeTestDataPath => @"ContextActions\AnnotateWithItemNotNull";

        [Test]
        public void TestExecuteJaggedArrayOnField() => DoNamedTest2();

        [Test]
        public void TestExecuteMultiDimensionalArrayOnField() => DoNamedTest2();

        [Test]
        public void TestExecuteArrayOnMethod() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericCollectionOnParameter() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericListOnProperty() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericListOnIndexer() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericListOnParameter() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericTaskOnMethod() => DoNamedTest2();

        [Test]
        public void TestExecuteLazyOnDelegate() => DoNamedTest2();

        [Test]
        public void TestExecuteGenericValueTaskOnMethod() => DoNamedTest2();
    }
}