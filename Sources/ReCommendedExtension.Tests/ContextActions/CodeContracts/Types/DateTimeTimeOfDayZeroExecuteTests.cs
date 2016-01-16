using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions.CodeContracts;

namespace ReCommendedExtension.Tests.ContextActions.CodeContracts.Types
{
    [TestNetFramework4]
    [TestFixture]
    public sealed class DateTimeTimeOfDayZeroExecuteTests : ContextActionExecuteTestBase<DateTimeTimeOfDayZero>
    {
        protected override string RelativeTestDataPath => @"ContextActions\CodeContracts\Types\DateTimeTimeOfDayZero";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}