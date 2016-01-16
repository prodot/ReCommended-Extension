using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class EnumFlagsExecuteTests : ContextActionExecuteTestBase<EnumFlags>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\EnumFlags";

        [Test]
        public void TestExecuteWithZero() => DoNamedTest2();

        [Test]
        public void TestExecuteWithoutZero() => DoNamedTest2();

        [Test]
        public void TestExecuteWithZeroSingleMember() => DoNamedTest2();
    }
}