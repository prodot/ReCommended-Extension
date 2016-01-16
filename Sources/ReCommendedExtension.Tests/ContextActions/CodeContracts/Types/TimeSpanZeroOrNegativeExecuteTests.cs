using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class TimeSpanZeroOrNegativeExecuteTests : ContextActionExecuteTestBase<TimeSpanZeroOrNegative>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\TimeSpanZeroOrNegative";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}