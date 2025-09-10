using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixTests : QuickFixTestBase<RemoveNotifyPropertyChangedInvocatorFix>
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor\QuickFixes";

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
}