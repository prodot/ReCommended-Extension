using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.DelegateInvoke;

namespace ReCommendedExtension.Tests.Analyzers.DelegateInvoke;

[TestFixture]
[TestNetFramework4]
public sealed class DelegateInvokeQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\DelegateInvoke\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is RedundantDelegateInvokeHint;

    [Test]
    public void TestDelegateInvokeAvailability() => DoNamedTest2();
}