using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class EnumKnownValuesExecuteTests : ContextActionExecuteTestBase<EnumKnownValues>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\EnumKnownValues";

        [Test]
        public void TestExecuteTwoMembers() => DoNamedTest2();

        [Test]
        public void TestExecuteFourMembers() => DoNamedTest2();
    }
}