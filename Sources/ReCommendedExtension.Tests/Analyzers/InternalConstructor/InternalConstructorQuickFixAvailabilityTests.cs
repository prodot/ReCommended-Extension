using JetBrains.ReSharper.Feature.Services.Daemon;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers.InternalConstructor;

[TestFixture]
public sealed class InternalConstructorQuickFixAvailabilityTests : QuickFixAvailabilityTests
{
    protected override string RelativeTestDataPath => @"Analyzers\InternalConstructor\QuickFixes";

    protected override bool UseHighlighting(IHighlighting highlighting) => highlighting is InternalConstructorVisibilitySuggestion;

    [Test]
    public void TestInternalConstructorAvailability() => DoNamedTest2();
}