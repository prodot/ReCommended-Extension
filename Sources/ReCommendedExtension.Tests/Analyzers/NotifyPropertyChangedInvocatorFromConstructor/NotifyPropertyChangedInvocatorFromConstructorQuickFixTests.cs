using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

namespace ReCommendedExtension.Tests.Analyzers.NotifyPropertyChangedInvocatorFromConstructor;

[TestFixture]
public sealed class NotifyPropertyChangedInvocatorFromConstructorQuickFixTests
    : QuickFixTestBase<NotifyPropertyChangedInvocatorFromConstructorWarning.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\NotifyPropertyChangedInvocatorFromConstructor\QuickFixes";

    [Test]
    public void TestNotifyPropertyChangedInvocatorFromConstructor() => DoNamedTest2();
}