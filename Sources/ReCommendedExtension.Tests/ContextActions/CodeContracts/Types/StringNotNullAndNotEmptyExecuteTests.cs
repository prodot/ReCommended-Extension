using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class StringNotNullAndNotEmptyExecuteTests : ContextActionExecuteTestBase<StringNotNullAndNotEmpty>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\StringNotNullAndNotEmpty";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}