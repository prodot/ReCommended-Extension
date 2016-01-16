using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class GuidNonEmptyExecuteTests : ContextActionExecuteTestBase<GuidNonEmpty>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\GuidNonEmpty";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}