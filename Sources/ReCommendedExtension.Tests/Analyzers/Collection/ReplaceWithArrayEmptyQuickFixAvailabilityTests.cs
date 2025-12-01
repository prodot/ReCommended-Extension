using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.TestFramework;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.Collection;

namespace ReCommendedExtension.Tests.Analyzers.Collection;

[TestFixture]
[CSharpLanguageLevel(CSharpLanguageLevel.CSharp110)]
[TestNetFramework46]
public sealed class ReplaceWithArrayEmptyQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\Collection\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is UseEmptyForArrayInitializationWarning;

    [Test]
    public void TestEmptyArrayInitializationAvailability() => DoNamedTest2();
}