using JetBrains.ReSharper.FeaturesTestFramework.Intentions;
using NUnit.Framework;
using ReCommendedExtension.Analyzers.InternalConstructor;

namespace ReCommendedExtension.Tests.Analyzers.InternalConstructor;

[TestFixture]
public sealed class InternalConstructorQuickFixTests : QuickFixTestBase<InternalConstructorVisibilitySuggestion.Fix>
{
    protected override string RelativeTestDataPath => @"Analyzers\InternalConstructor\QuickFixes";

    [Test]
    public void InternalConstructorToProtected() => DoNamedTest();

    [Test]
    public void InternalConstructorToPrivateProtected() => DoNamedTest();
}