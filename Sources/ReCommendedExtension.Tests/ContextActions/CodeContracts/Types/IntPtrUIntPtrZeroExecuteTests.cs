using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class IntPtrUIntPtrZeroExecuteTests : ContextActionExecuteTestBase<IntPtrUIntPtrZero>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\IntPtrUIntPtrZero";

        [Test]
        public void TestExecuteIntPtr() => DoNamedTest2();

        [Test]
        public void TestExecuteUIntPtr() => DoNamedTest2();
    }
}