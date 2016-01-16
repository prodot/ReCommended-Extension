using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.QuickFixes;

namespace ReCommendedExtension.Tests.Analyzers
{
    [TestFixture]
    public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixTests : QuickFixTestBase<RemoveNotifyPropertyChangedInvocatorFix>
    {
        protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructorQuickFixes";

        [Test]
        public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
    }
}