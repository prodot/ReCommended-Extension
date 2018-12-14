using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.ContextActions;

namespace ReCommendedExtension.Tests.ContextActions
{
    [TestNetFramework45]
    [TestFixture]
    public sealed class AddConfigureAwaitExecuteTests : CSharpContextActionExecuteTestBase<AddConfigureAwait>
    {
        protected override string ExtraPath => "";

        protected override string RelativeTestDataPath => @"ContextActions\AddConfigureAwait";

        [Test]
        public void TestExecute() => DoNamedTest2();
    }
}